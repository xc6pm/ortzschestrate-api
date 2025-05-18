using Chess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Ortzschestrate.Api.Models;

namespace Ortzschestrate.Api.Hubs;

[Authorize]
public partial class GameHub
{
    [HubMethodName("create")]
    public async Task CreateGameAsync(int time, char creatorColor, double stakeEth)
    {
        if (!TimeControl.TryFromValue(time, out TimeControl timeLimit))
            throw new HubException("The gameType argument is invalid.");

        if (stakeEth > 0)
        {
            var user = await userManager.FindByIdAsync(Context.UserIdentifier!);
            if (String.IsNullOrEmpty(user.WalletAddress))
            {
                throw new HubException("You need a verified wallet address to start a wagered game.");
            }
        }

        var color = creatorColor switch
        {
            'w' => PieceColor.White,
            'b' => PieceColor.Black,
            _ => throw new HubException("The creatorColor argument is invalid.")
        };

        await _lobbySemaphore.WaitAsync();
        try
        {
            var player = playerCache.GetPlayer(Context.UserIdentifier!);
            if (player.OngoingShortGame != null)
            {
                throw new HubException("You're already playing a game. Finish that first.");
            }

            _pendingGamesByCreatorId.TryAdd(player.UserId,
                new PendingGame(player, timeLimit, color, stakeEth));
        }
        finally
        {
            _lobbySemaphore.Release();
        }

        await Clients.All.LobbyUpdated(_pendingGamesByCreatorId.Values.ToList());
    }

    [HubMethodName("getPending")]
    public List<PendingGame> GetAllPendingGamesAsync() => _pendingGamesByCreatorId.Values.ToList();

    [HubMethodName("cancel")]
    public async Task CancelPendingGameAsync()
    {
        await _lobbySemaphore.WaitAsync();
        bool removedAnything;
        try
        {
            removedAnything = _pendingGamesByCreatorId.TryRemove(Context.UserIdentifier!, out _);
        }
        finally
        {
            _lobbySemaphore.Release();
        }

        if (removedAnything)
            await Clients.All.LobbyUpdated(_pendingGamesByCreatorId.Values.ToList());
    }


    [HubMethodName("join")]
    // Checks for:
    // - Does that creator connection even exist. -- must be done in semaphore, we check whether it's in waitingPlayers.
    // - Is the creator the same connection/user that wants to join. -- that's invalid can be checked outside semaphore.
    // - Does the user have a pending game -- inside semaphore for updates to take place. user must cancel the pending game first
    // - Is the user already playing another game. -- inside the semaphore we must ensure we apply the latest updates. Only one game per user is allowed rn
    public async Task<string> JoinGameAsync(string creatorId)
    {
        if (creatorId == Context.UserIdentifier!)
        {
            throw new HubException("Can't join your own game.");
        }

        bool wageredGame = _pendingGamesByCreatorId.TryGetValue(creatorId, out var g) && g.StakeEth > 0;
        string? player1Address = null, player2Address = null;
        if (wageredGame)
        {
            var players = await userManager.Users
                .Where(u => u.Id == creatorId || u.Id == Context.UserIdentifier!)
                .Take(2).ToListAsync();
            player1Address = players.First(p => p.Id == creatorId).WalletAddress!;
            player2Address = players.First(p => p.Id == Context.UserIdentifier!).WalletAddress;

            if (String.IsNullOrEmpty(player2Address))
            {
                throw new HubException("You need a verified wallet address to join a wagered game.");
            }
        }


        Models.Game? startingGame = null;
        await _lobbySemaphore.WaitAsync();
        PendingGame pendingGame = null;
        Player? player1 = null, player2 = null;
        try
        {
            player1 = playerCache.GetPlayer(creatorId);
            player2 = playerCache.GetPlayer(Context.UserIdentifier!);
            if (_pendingGamesByCreatorId.ContainsKey(Context.UserIdentifier!))
            {
                throw new HubException("Cancel the game you created first.");
            }

            if (player2.OngoingShortGame != null)
            {
                throw new HubException("You're already playing a game. Finish that first.");
            }

            if (_pendingGamesByCreatorId.TryRemove(creatorId, out pendingGame))
            {
                startingGame = new Models.Game(pendingGame, player2);
                player1.OngoingShortGame = startingGame;
                player2.OngoingShortGame = startingGame;
                _ongoingShortGames[startingGame.Id] = startingGame;
                return startingGame.Id.ToString();
            }
            else // Player1 already started another game.
            {
                throw new HubException($"Couldn't join that game. Perhaps it's been joined to by another player.");
            }
        }
        finally
        {
            _lobbySemaphore.Release();

            // Want to execute these tasks outside the semaphore and before the return.
            if (startingGame != null)
            {
                if (wageredGame)
                {
                    var gameStartedOnBlockchain = await startGame.DoAsync(startingGame.Id, player1Address!,
                        player2Address!,
                        startingGame.StakeEth);

                    if (!gameStartedOnBlockchain)
                    {
                        player1!.OngoingShortGame = null;
                        player2!.OngoingShortGame = null;
                        _ongoingShortGames.TryRemove(startingGame.Id, out _);
                        _pendingGamesByCreatorId.TryAdd(creatorId, pendingGame!);
                        throw new HubException("Reverted due to Blockchain error.");
                    }
                }

                await Clients.All.LobbyUpdated(_pendingGamesByCreatorId.Values.ToList());
                _ = outgoingMessageTracker.GameStartedAsync(startingGame.Players[0].UserId, startingGame.Id.ToString());
            }
        }
    }
}
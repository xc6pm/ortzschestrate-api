using Chess;
using Microsoft.AspNetCore.SignalR;
using Ortzschestrate.Api.Models;

namespace Ortzschestrate.Api.Hubs;

public partial class GameHub
{
    [HubMethodName("create")]
    public async Task CreateGameAsync(int time, char creatorColor)
    {
        if (!GameType.TryFromValue(time, out GameType timeLimit))
            throw new HubException("The gameType argument is invalid.");

        var color = creatorColor switch
        {
            'w' => PieceColor.White,
            'b' => PieceColor.Black,
            _ => throw new HubException("The creatorColor argument is invalid.")
        };

        await _lobbySemaphore.WaitAsync();
        try
        {
            var player = _playerCache.GetPlayer(Context.UserIdentifier!);
            if (player.OngoingShortGame != null)
            {
                throw new HubException("You're already playing a game. Finish that first.");
            }

            _pendingGamesByCreatorId.TryAdd(player.UserId,
                new PendingGame(player, timeLimit, color));
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

        Models.Game? startingGame = null;
        await _lobbySemaphore.WaitAsync();
        try
        {
            var player1 = _playerCache.GetPlayer(creatorId);
            var player2 = _playerCache.GetPlayer(Context.UserIdentifier!);
            if (_pendingGamesByCreatorId.ContainsKey(Context.UserIdentifier!))
            {
                throw new HubException("Cancel the game you created first.");
            }

            if (player2.OngoingShortGame != null)
            {
                throw new HubException("You're already playing a game. Finish that first.");
            }

            if (_pendingGamesByCreatorId.TryRemove(creatorId, out var pendingGame))
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
                await Clients.All.LobbyUpdated(_pendingGamesByCreatorId.Values.ToList());
                await Clients.User(startingGame.Player1.UserId)
                    .GameStarted(startingGame.Id.ToString());
            }
        }
    }
}
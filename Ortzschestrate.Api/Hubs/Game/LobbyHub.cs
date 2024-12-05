using Chess;
using Microsoft.AspNetCore.SignalR;
using Ortzschestrate.Api.Models;

namespace Ortzschestrate.Api.Hubs;

public partial class GameHub
{
    [HubMethodName("create")]
    public async Task CreateGameAsync(string gameType, string creatorColor)
    {
        if (!GameType.TryFromName(gameType, out GameType timeLimit))
            throw new HubException("The gameType argument is invalid.");

        if (!PieceColor.TryFromName(creatorColor, out PieceColor color))
            throw new HubException("The creatorColor argument is invalid.");

        await _lobbySemaphore.WaitAsync();
        try
        {
            var player = _playerCache.GetPlayer(Context.UserIdentifier!);
            if (player.OngoingGamesByConnectionId.ContainsKey(Context.ConnectionId))
            {
                throw new HubException("You're already in the middle of a game.");
            }

            _pendingGamesByCreatorConnectionId.TryAdd(Context.ConnectionId,
                new PendingGame(player, Context.ConnectionId, timeLimit, color));
        }
        finally
        {
            _lobbySemaphore.Release();
        }

        await Clients.All.LobbyUpdated(_pendingGamesByCreatorConnectionId.Values.ToList());
    }

    [HubMethodName("cancel")]
    public async Task CancelPendingGameAsync(string creatorConnectionId)
    {
        await _lobbySemaphore.WaitAsync();
        try
        {
            var player = _playerCache.GetPlayer(Context.UserIdentifier!);
            // Ensure it's the user who created it who's trying to cancel. 
            if (_pendingGamesByCreatorConnectionId.TryGetValue(creatorConnectionId, out PendingGame? pendingGame) &&
                pendingGame.Creator.UserId == player.UserId)
            {
                _pendingGamesByCreatorConnectionId.TryRemove(Context.ConnectionId, out _);
            }
            else
            {
                throw new HubException("You can't cancel that game.");
            }
        }
        finally
        {
            _lobbySemaphore.Release();
        }

        await Clients.All.LobbyUpdated(_pendingGamesByCreatorConnectionId.Values.ToList());
    }

    [HubMethodName("join")]
    // Checks for:
    // - Does that creator connection even exist. -- must be done in semaphore, we check whether it's in waitingPlayers.
    // - Is the creator the same connection/user that wants to join. -- that's invalid can be checked outside semaphore.
    // - Does the user have a pending game -- inside semaphore for updates to take place. user must cancel the pending game first
    // - Is the user already playing another game. -- inside the semaphore we must ensure we apply the latest updates. Only one game per user is allowed rn
    public async Task<string> JoinGameAsync(string creatorId, string creatorConnectionId)
    {
        if (creatorConnectionId == Context.ConnectionId)
        {
            throw new HubException("Can't join your own game.");
        }

        var player1 = _playerCache.GetPlayer(creatorId);
        var player2 = _playerCache.GetPlayer(Context.UserIdentifier!);

        if (player1.UserId == player2.UserId)
        {
            throw new HubException("Can't join your own game.");
        }

        Models.Game? startingGame = null;
        await _lobbySemaphore.WaitAsync();
        try
        {
            // These may be overwritten while waiting for the semaphore so get them again.
            player1 = _playerCache.GetPlayer(creatorId);
            player2 = _playerCache.GetPlayer(Context.UserIdentifier!);
            if (_pendingGamesByCreatorConnectionId.ContainsKey(Context.ConnectionId))
            {
                throw new HubException("Cancel the game you created first.");
            }

            if (player2.OngoingGamesByConnectionId.ContainsKey(Context.ConnectionId))
            {
                throw new HubException("You're already in the middle of a game.");
            }

            if (_pendingGamesByCreatorConnectionId.TryRemove(creatorConnectionId, out var pendingGame))
            {
                startingGame = new Models.Game(pendingGame, player2, Context.ConnectionId);
                player1.OngoingGamesByConnectionId[creatorConnectionId] = startingGame;
                player2.OngoingGamesByConnectionId[Context.ConnectionId] = startingGame;
                _games[startingGame.Id] = startingGame;
                return startingGame.Id;
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
                await Groups.AddToGroupAsync(creatorConnectionId, $"game_{startingGame.Id}");                
                await Groups.AddToGroupAsync(Context.ConnectionId, $"game_{startingGame.Id}");
                await Clients.All.LobbyUpdated(_pendingGamesByCreatorConnectionId.Values.ToList());
                await Clients.User(startingGame.Player1.UserId)
                    .GameStarted(startingGame.Id);
            }
        }
    }
}
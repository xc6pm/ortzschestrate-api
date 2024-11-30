using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Ortzschestrate.Api.Models;
using Ortzschestrate.Api.Security;
using Ortzschestrate.Data.Models;

namespace Ortzschestrate.Api.Hubs;

public partial class GameHub : Hub
{
    [HubMethodName("create")]
    public async Task CreateGameAsync()
    {
        await Clients.All.SendAsync("NewGameCreated", Context.User!.Identity!.Name);

        await _lobbySemaphore.WaitAsync();
        try
        {
            if (_connections.TryGetValue(Context.ConnectionId, out var player))
            {
                if (player.GameId != null)
                {
                    throw new HubException("You're already in the middle of a game.");
                }
            }
            else
            {
                player = await createPlayerFromCurrentConnectionAsync();
            }

            _connections[Context.ConnectionId] = player;

            _waitingPlayers.TryAdd(Context.User.FindId(), player);
        }
        finally
        {
            _lobbySemaphore.Release();
        }

        await Clients.All.SendAsync("LobbyUpdated", _waitingPlayers.Values.ToList());
    }

    [HubMethodName("cancel")]
    public async Task CancelPendingGameAsync()
    {
        await _lobbySemaphore.WaitAsync();
        try
        {
            _waitingPlayers.TryRemove(Context.User.FindId(), out _);
        }
        finally
        {
            _lobbySemaphore.Release();
        }

        await Clients.All.SendAsync("LobbyUpdated", _waitingPlayers.Values.ToList());
    }

    [HubMethodName("join")]
    // Checks for:
    // - Does that creator connection even exist. -- must be done in semaphore, we check whether it's in waitingPlayers.
    // - Is the creator the same connection/user that wants to join. -- that's invalid can be checked outside semaphore.
    // - Does the user have a pending game -- inside semaphore for updates to take place. user must cancel the pending game first
    // - Is the user already playing another game. -- inside the semaphore we must ensure we apply the latest updates. Only one game per user is allowed rn
    public async Task<Game> JoinGameAsync(string creatorConnectionId)
    {
        if (creatorConnectionId == Context.ConnectionId)
        {
            throw new HubException("Can't join your own game.");
        }

        var player1 = _connections[creatorConnectionId];
        var player2 = await getOrCreateCurrentPlayerAsync();

        if (player1.UserId == player2.UserId)
        {
            throw new HubException("Can't join your own game.");
        }

        Game? startingGame = null;
        await _lobbySemaphore.WaitAsync();
        try
        {
            // These may be overwritten while waiting for the semaphore so get them again.
            player1 = _connections[creatorConnectionId];
            player2 = await getOrCreateCurrentPlayerAsync();
            if (_waitingPlayers.ContainsKey(Context.User!.FindId()))
            {
                throw new HubException("Cancel the game you created first.");
            }

            if (_connections.TryGetValue(Context.ConnectionId, out player2) &&
                player2.GameId != null)
            {
                throw new HubException("You're already in the middle of a game.");
            }

            if (_waitingPlayers.TryRemove(player1.UserId, out player1))
            {
                startingGame = new Game(player1, player2);
                player1.GameId = player2.GameId = startingGame.Id;
                _games[startingGame.Id] = startingGame;
                return startingGame;
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
                await Clients.All.SendAsync("LobbyUpdated", _waitingPlayers.Values.ToList());
                await Clients.User(startingGame.Player1.UserId).SendAsync("GameStarted", startingGame);
            }
        }
    }

    private async Task<Player> getOrCreateCurrentPlayerAsync()
    {
        if (_connections.TryGetValue(Context.ConnectionId, out var player))
        {
            return player;
        }

        player = await createPlayerFromCurrentConnectionAsync();
        _connections[Context.ConnectionId] = player;
        return player;
    }

    private async Task<Player> createPlayerFromCurrentConnectionAsync()
    {
        var userManager = _serviceProvider.GetRequiredService<UserManager<User>>();

        var userId = Context.User!.FindId();
        var userFromDb = await userManager.FindByIdAsync(userId);

        var player = new Player(userId, userFromDb!.UserName!);

        return player;
    }
}
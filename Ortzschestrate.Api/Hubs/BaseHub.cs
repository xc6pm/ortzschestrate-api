using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using Ortzschestrate.Api.Models;
using Ortzschestrate.Api.Utilities;

namespace Ortzschestrate.Api.Hubs;

public partial class GameHub : Hub
{
    private static readonly List<Player> _waitingPlayers = new();
    private static readonly ConcurrentDictionary<string, Player> _players = new();
    private static readonly ConcurrentDictionary<string, Game> _games = new();
    private static readonly SemaphoreSlim lobbySemaphore = new(1, 1);

    private readonly IServiceProvider _serviceProvider;

    public GameHub(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        Debug.WriteLine($"New client connected");
        await Clients.All.SendAsync("PlayerJoinedLobby", Context.User!.GetSubClaim());
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        bool lobbyUpdated = false;
        await lobbySemaphore.WaitAsync();
        try
        {
            if (_players.TryGetValue(Context.ConnectionId, out var leavingPlayer))
            {
                if (leavingPlayer.GameId != null)
                {
                    // Finish the game, if more than 10 moves were made the abandoning player must lose. 
                }
                else
                {
                    _waitingPlayers.Remove(leavingPlayer);
                    lobbyUpdated = true;
                }

                _players.TryRemove(Context.ConnectionId, out _);
            }
        }
        finally
        {
            lobbySemaphore.Release();
            if (lobbyUpdated)
                await Clients.All.SendAsync("LobbyUpdated", _waitingPlayers.ToList());
        }

        Debug.WriteLine("Connection closed");
        Debug.WriteLine(exception);
        await base.OnDisconnectedAsync(exception);
    }
}
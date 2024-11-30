using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Ortzschestrate.Api.Models;
using Ortzschestrate.Api.Security;

namespace Ortzschestrate.Api.Hubs;

[Authorize]
public partial class GameHub : Hub
{
    private static readonly ConcurrentDictionary<string, Player> _waitingPlayers = new();
    private static readonly ConcurrentDictionary<string, Player> _connections = new();
    private static readonly ConcurrentDictionary<string, Game> _games = new();
    private static readonly SemaphoreSlim _lobbySemaphore = new(1, 1);

    private readonly IServiceProvider _serviceProvider;

    public GameHub(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        Debug.WriteLine($"New client connected");
        await Clients.All.SendAsync("PlayerJoinedLobby", Context.User!.FindId());
        await Clients.Caller.SendAsync("LobbyUpdated", _connections.Values.ToList());
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        bool lobbyUpdated = false;
        
        await _lobbySemaphore.WaitAsync();
        try
        {
            if (_connections.TryGetValue(Context.ConnectionId, out var leavingPlayer))
            {
                if (leavingPlayer.GameId != null)
                {
                    // Finish the game, if more than 10 moves were made the abandoning player must lose. 
                }
                else
                {
                    _waitingPlayers.TryRemove(Context.User!.FindId(), out _);
                    lobbyUpdated = true;
                }

                _connections.TryRemove(Context.ConnectionId, out _);
            }
        }
        finally
        {
            _lobbySemaphore.Release();
            if (lobbyUpdated)
                await Clients.All.SendAsync("LobbyUpdated", _waitingPlayers.Values.ToList());
        }

        Debug.WriteLine("Connection closed");
        Debug.WriteLine(exception);
        await base.OnDisconnectedAsync(exception);
    }
}
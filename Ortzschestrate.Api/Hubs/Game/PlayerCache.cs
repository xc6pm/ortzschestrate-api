using System.Collections.Concurrent;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Ortzschestrate.Api.Models;
using Ortzschestrate.Data.Models;

namespace Ortzschestrate.Api.Hubs.Game;

/// <summary>
/// 
/// </summary>
///
/// Use cases:
/// - We need to keep one player object for all their connections so we can prevent users from creating/joining
/// unboundedly many games.
/// - The cache also manages the number of players we cache so that disconnected players are not kept in memory
/// needlessly.
public class PlayerCache
{
    private readonly ConcurrentDictionary<string, Player> _playersById = new();
    private readonly ConcurrentDictionary<string, List<string>> _playerConnections = new();

    public Player GetPlayer(string playerId)
    {
        return _playersById[playerId];
    }

    public async Task OnNewConnectionAsync(HubCallerContext context)
    {
        if (_playersById.TryGetValue(context.UserIdentifier!, out _))
        {
            _playerConnections[context.UserIdentifier!].Add(context.ConnectionId);
            return;
        }

        var userManager = context.GetHttpContext()!.RequestServices.GetRequiredService<UserManager<User>>();
        var user = await userManager.FindByIdAsync(context.UserIdentifier!);

        if (user == null)
            throw new ArgumentException("Invalid userId.");

        var newPlayer = new Player(context.UserIdentifier!, user.UserName!);
        _playersById.TryAdd(context.UserIdentifier!, newPlayer);
        _playerConnections.TryAdd(context.UserIdentifier!, []);
    }

    public void OnDisconnect(HubCallerContext context)
    {
        _playerConnections.TryGetValue(context.UserIdentifier!, out var connections);
        connections!.Remove(context.ConnectionId);
        if (connections.Count == 0)
        {
            _playerConnections.Remove(context.UserIdentifier!, out _);
            _playersById.Remove(context.UserIdentifier!, out _);
        }
    }
}
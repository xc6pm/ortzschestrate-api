using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.JsonWebTokens;
using Ortzschestrate.Api.Models;
using Ortzschestrate.Api.Utilities;
using Ortzschestrate.Data.Models;

namespace Ortzschestrate.Api.Hubs;

public partial class GameHub : Hub
{
    // Must require authorization
    [HubMethodName("create")]
    public async Task CreateGameAsync()
    {
        await Clients.All.SendAsync("NewGameCreated",
            Context.User.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);

        var userManager = _serviceProvider.GetRequiredService<UserManager<User>>();

        var userId = Context.User.GetSubClaim();
        var userFromDb = await userManager.FindByIdAsync(userId);

        var player = new Player(Context.ConnectionId, userId, userFromDb!.UserName!);

        await lobbySemaphore.WaitAsync();
        try
        {
            _players[Context.ConnectionId] = player;
        
            _waitingPlayers.Add(player);
        }
        finally
        {
            lobbySemaphore.Release();
        }

        await Clients.All.SendAsync("LobbyUpdated", _waitingPlayers.ToList());
    }
}
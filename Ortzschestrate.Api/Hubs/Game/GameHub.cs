using Microsoft.AspNetCore.SignalR;
using Ortzschestrate.Api.Security;

namespace Ortzschestrate.Api.Hubs;

public partial class GameHub
{
    [HubMethodName("move")]
    public Models.Game MoveAsync(string gameId, string move)
    {
        if (string.IsNullOrWhiteSpace(gameId) || string.IsNullOrWhiteSpace(move))
            throw new HubException("GameId and move must be given.");

        if (!_games.TryGetValue(gameId, out var game))
            throw new HubException($"A game with id {gameId} doesn't exist.");

        bool success;
        try
        {
            success = game.Move(Context.User!.FindId(), move);
        }
        catch (ArgumentException e)
        {
            throw new HubException(e.Message);
        }
        if (!success)
            throw new HubException("Couldn't make that move.");

        return game;
    }
}
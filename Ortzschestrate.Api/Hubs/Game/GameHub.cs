using Microsoft.AspNetCore.SignalR;
using Ortzschestrate.Api.Security;

namespace Ortzschestrate.Api.Hubs;

public partial class GameHub
{
    [HubMethodName("getGame")]
    public object GetOngoingGame(string gameId)
    {
        var game = _games[gameId];
        if (game.Player1ConnectionId == Context.ConnectionId)
        {
            return new
            {
                Color = game.Player1Color.AsChar, Opponent = game.Player2.Name,
                TimeInMilliseconds = game.GameType.GetTimeSpan().TotalMilliseconds
            };
        }

        if (game.Player2ConnectionId == Context.ConnectionId)
        {
            return new
            {
                Color = game.Player2Color.AsChar, Opponent = game.Player1.Name,
                TimeInMilliseconds = game.GameType.GetTimeSpan().TotalMilliseconds
            };
        }

        throw new HubException("Couldn't find that game.");
    }

    [HubMethodName("move")]
    public async Task MoveAsync(string gameId, string move)
    {
        if (string.IsNullOrWhiteSpace(gameId) || string.IsNullOrWhiteSpace(move))
            throw new HubException("GameId and move must be given.");

        if (!_games.TryGetValue(gameId, out var game))
            throw new HubException($"A game with id {gameId} doesn't exist.");

        bool success;
        TimeSpan remainingTime;

        try
        {
            success = game.Move(Context.User!.FindId(), move, out remainingTime);
        }
        catch (ArgumentException e)
        {
            throw new HubException(e.Message);
        }

        if (!success)
            throw new HubException("Couldn't make that move.");

        await Clients.Group($"game_{gameId}").PlayerMoved(new(move, remainingTime.TotalMilliseconds));

        if (game.EndGame != null)
        {
            await Clients.Group($"game_{gameId}")
                .GameEnded(new(game.EndGame.EndgameType.ToString(), game.EndGame.WonSide?.AsChar));
        }
    }
}
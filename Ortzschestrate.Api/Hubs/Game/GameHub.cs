using Microsoft.AspNetCore.SignalR;
using Ortzschestrate.Api.Security;

namespace Ortzschestrate.Api.Hubs;

public partial class GameHub
{
    [HubMethodName("getGame")]
    public object? GetOngoingGame(string gameId)
    {
        if (!_ongoingGames.TryGetValue(gameId, out var game))
        {
            return null;
        }

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

        return null;
    }

    [HubMethodName("move")]
    public async Task MoveAsync(string gameId, string move)
    {
        if (string.IsNullOrWhiteSpace(gameId) || string.IsNullOrWhiteSpace(move))
            throw new HubException("GameId and move must be given.");

        if (!_ongoingGames.TryGetValue(gameId, out var game))
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
            await declareGameEndedAsync(game);
        }
    }

    [HubMethodName("timeout")]
    public async Task<bool> ReportTimeoutAsync(string gameId)
    {
        var game = _ongoingGames[gameId];

        if (game.IsPlayer1OutOfTime())
        {
            await declareGameEndedAsync(game);
            return true;
        }

        if (game.IsPlayer2OutOfTime())
        {
            await declareGameEndedAsync(game);
            return true;
        }

        return false;
    }

    private async Task declareGameEndedAsync(Models.Game game)
    {
        await Clients.Group($"game_{game.Id}")
            .GameEnded(new(game.EndGame!.EndgameType.ToString(), game.EndGame.WonSide?.AsChar));
        game.Player1.OngoingGamesByConnectionId.TryRemove(game.Player1ConnectionId, out _);
        game.Player2.OngoingGamesByConnectionId.TryRemove(game.Player2ConnectionId, out _);
        _ongoingGames.TryRemove($"game_{game.Id}", out _);
    }
}
using Microsoft.AspNetCore.SignalR;

namespace Ortzschestrate.Api.Hubs;

public partial class GameHub
{
    [HubMethodName("getGame")]
    public object? GetOngoingGame(Guid gameId)
    {
        if (!_ongoingShortGames.TryGetValue(gameId, out var game))
        {
            return null;
        }

        if (game.Player1.UserId == Context.UserIdentifier!)
        {
            return new
            {
                Color = game.Player1Color.AsChar, Opponent = game.Player2.Name,
                TimeInMilliseconds = game.GameType.GetTimeSpan().TotalMilliseconds
            };
        }

        if (game.Player2.UserId == Context.UserIdentifier!)
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
    public async Task MoveAsync(Guid gameId, string move)
    {
        if (gameId == Guid.Empty || string.IsNullOrWhiteSpace(move))
            throw new HubException("GameId and move must be given.");

        if (!_ongoingShortGames.TryGetValue(gameId, out var game))
            throw new HubException($"A game with id {gameId} doesn't exist.");

        bool success;
        TimeSpan remainingTime;

        try
        {
            success = game.Move(Context.UserIdentifier!, move, out remainingTime);
        }
        catch (ArgumentException e)
        {
            throw new HubException(e.Message);
        }

        if (!success)
            throw new HubException("Couldn't make that move.");

        await Clients.Users([game.Player1.UserId, game.Player2.UserId])
            .PlayerMoved(new(move, remainingTime.TotalMilliseconds));

        if (game.EndGame != null)
        {
            await disconnectEndedGameAsync(game);
        }
    }

    [HubMethodName("timeout")]
    public async Task<bool> ReportTimeoutAsync(Guid gameId)
    {
        var game = _ongoingShortGames[gameId];

        if (game.IsPlayer1OutOfTime())
        {
            await disconnectEndedGameAsync(game);
            return true;
        }

        if (game.IsPlayer2OutOfTime())
        {
            await disconnectEndedGameAsync(game);
            return true;
        }

        return false;
    }
}
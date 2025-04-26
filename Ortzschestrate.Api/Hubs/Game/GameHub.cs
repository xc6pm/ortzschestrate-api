using Microsoft.AspNetCore.SignalR;
using Ortzschestrate.Api.Models;

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

        int playerIdx;
        try
        {
            playerIdx = game.GetPlayerIdx(Context.UserIdentifier!);
        }
        catch (ArgumentException e)
        {
            throw new HubException(e.Message);
        }

        return new OngoingGame(game, playerIdx);
    }

    [HubMethodName("ongoingShortGame")]
    public object? GetOngoingShortGameAsync()
    {
        var player = playerCache.GetPlayer(Context.UserIdentifier!);

        if (player.OngoingShortGame == null)
            return null;

        int playerIdx = player.OngoingShortGame.GetPlayerIdx(player.UserId);
        return new OngoingGame(player.OngoingShortGame, playerIdx);
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

        _ = outgoingMessageTracker.PlayerMovedAsync(game.Players[0].UserId,
            new GameUpdate(move, remainingTime.TotalMilliseconds));
        _ = outgoingMessageTracker.PlayerMovedAsync(game.Players[1].UserId,
            new GameUpdate(move, remainingTime.TotalMilliseconds));

        if (game.EndGame != null)
        {
            await finalizeEndedGameAsync(game);
        }
    }

    [HubMethodName("timeout")]
    public async Task<bool> ReportTimeoutAsync(Guid gameId)
    {
        var game = _ongoingShortGames[gameId];

        if (game.IsPlayer1OutOfTime())
        {
            await finalizeEndedGameAsync(game);
            return true;
        }

        if (game.IsPlayer2OutOfTime())
        {
            await finalizeEndedGameAsync(game);
            return true;
        }

        return false;
    }

    [HubMethodName("resignShortGame")]
    public async Task ResignOngoingShortGameAsync()
    {
        var player = playerCache.GetPlayer(Context.UserIdentifier!);

        if (player.OngoingShortGame == null)
            throw new HubException("You don't have a short game in progress.");

        player.OngoingShortGame.Resign(player);

        await finalizeEndedGameAsync(player.OngoingShortGame);
    }
}
namespace Ortzschestrate.Api.Models;

public record OngoingGame(
    string Id,
    string Opponent,
    char Color,
    double TimeInMilliseconds,
    double PlayerRemainingTime,
    double OpponentRemainingTime,
    double StakeEth,
    string Pgn,
    IReadOnlyList<string> MovesPlayed)
{
    public OngoingGame(Game game, int playerIdx) : this(
        game.Id.ToString(),
        game.Players[playerIdx == 0 ? 1 : 0].Name,
        game.PlayerColors[playerIdx].AsChar,
        game.TimeControl.GetTimeSpan().TotalMilliseconds,
        game.CalculateRemainingTime(playerIdx).TotalMilliseconds,
        game.CalculateRemainingTime(playerIdx == 0 ? 1 : 0).TotalMilliseconds,
        game.StakeEth,
        game.Pgn,
        game.MovesPlayed)
    {
    }

    public void Deconstruct(out string Id, out string Opponent, out char Color, out double TimeInMilliseconds,
        out double PlayerRemainingTime, out double OpponentRemainingTime, out double StakeEth, out string Pgn,
        out IReadOnlyList<string> MovesPlayed)
    {
        Id = this.Id;
        Opponent = this.Opponent;
        Color = this.Color;
        TimeInMilliseconds = this.TimeInMilliseconds;
        PlayerRemainingTime = this.PlayerRemainingTime;
        OpponentRemainingTime = this.OpponentRemainingTime;
        StakeEth = this.StakeEth;
        Pgn = this.Pgn;
        MovesPlayed = this.MovesPlayed;
    }
}
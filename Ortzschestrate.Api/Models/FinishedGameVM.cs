using Ortzschestrate.Data.Models;

namespace Ortzschestrate.Api.Models;

public record FinishedGameSlim(
    string Id,
    Player[] Players,
    char[] PlayerColors,
    double StakeEth,
    TimeControl TimeControl,
    DateTime Started,
    char? WonSide)
{
    public FinishedGameSlim(FinishedGame g) : this(g.Id.ToString(),
        g.Players.Select(p => new Player(p.Id, p.UserName!)).ToArray(),
        g.PlayerColors.Select(c => c == Color.White ? 'w' : 'b').ToArray(),
        g.StakeEth,
        TimeControl.FromMilliseconds(g.TimeInMs),
        g.Started,
        g.WonSide switch
        {
            null => null,
            Color.White => 'w',
            _ => 'b'
        })
    {
    }
}

public record FinishedGameVM(
    string Id,
    Player[] Players,
    char[] PlayerColors,
    double StakeEth,
    TimeControl TimeControl,
    DateTime Started,
    double[] RemainingTimesInMs,
    string Pgn,
    string EndgameType,
    char? WonSide
)
{
    public FinishedGameVM(FinishedGame g) : this(g.Id.ToString(),
        g.Players.Select(p => new Player(p.Id, p.UserName!)).ToArray(),
        g.PlayerColors.Select(c => c == Color.White ? 'w' : 'b').ToArray(),
        g.StakeEth,
        TimeControl.FromMilliseconds(g.TimeInMs),
        g.Started,
        g.RemainingTimesInMs.ToArray(),
        g.Pgn,
        g.EndGameType.ToString(),
        g.WonSide switch
        {
            null => null,
            Color.White => 'w',
            _ => 'b'
        })
    {
    }
}
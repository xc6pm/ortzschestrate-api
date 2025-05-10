using Ortzschestrate.Data.Models;

namespace Ortzschestrate.Api.Models;

public record FinishedGameVM(
    string Id,
    Player[] Players,
    char[] PlayerColors,
    double StakeEth,
    double TimeInMs,
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
        g.TimeInMs,
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
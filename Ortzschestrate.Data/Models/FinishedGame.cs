namespace Ortzschestrate.Data.Models;

public class FinishedGame
{
    public required Guid Id { get; init; }

    public required List<User> Players { get; init; }
    
    public required Color[] PlayerColors { get; init; }

    public required double StakeEth { get; init; }

    public required double TimeInMs { get; init; }

    public required DateTime Started { get; init; }

    public required double[] RemainingTimesInMs { get; init; }

    public required string Pgn { get; init; }

    public required EndgameType EndGameType { get; init; }

    public required Color? WonSide { get; init; }
}
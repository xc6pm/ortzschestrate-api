namespace Ortzschestrate.Api.Models;

public record FinishedGame(
    string Id,
    Player[] Players,
    char[] PlayerColors,
    double StakeEth,
    double TimeInMs,
    DateTime Started,
    double[] RemainingTimesInMs,
    string Pgn,
    string EndgameType,
    char? WonSide);
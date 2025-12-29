using Chess;
using Microsoft.AspNetCore.SignalR;

namespace Ortzschestrate.Api.Models;

// Terminology: Player1 = Players[0] & Player2 = Players[1]
public class Game
{
    private readonly ChessBoard _board = new();
    private DateTime _lastMoveTime;

    public Game(PendingGame pendingGame, Player player2)
    {
        Players = [pendingGame.Creator, player2];
        PlayerColors =
        [
            pendingGame.CreatorColor,
            pendingGame.CreatorColor == PieceColor.White ? PieceColor.Black : PieceColor.White
        ];
        StakeEth = pendingGame.StakeEth;
        TimeControl = pendingGame.TimeControl;
        Id = Guid.NewGuid();
        StartedTime = _lastMoveTime = DateTime.UtcNow;
        RemainingTimes = [pendingGame.TimeControl.GetTimeSpan(), pendingGame.TimeControl.GetTimeSpan()];
    }

    public Guid Id { get; }

    public Player[] Players { get; }
    public PieceColor[] PlayerColors { get; }
    public double StakeEth { get; }
    public bool IsWagered => StakeEth > 0;
    public TimeControl TimeControl { get; }
    public DateTime StartedTime { get; }
    public TimeSpan[] RemainingTimes { get; }
    public CancellationTokenSource?[] ConnectionTimeoutCancellations { get; } = new CancellationTokenSource[2];
    public bool IsPlayer1Turn => _board.Turn == PlayerColors[0];
    public PieceColor Turn => _board.Turn;
    public PieceColor LastTurn => _board.ExecutedMoves[^1].Piece.Color;


    public string Pgn => _board.ToPgn();
    public string Fen => _board.ToFen();
    public IReadOnlyList<string> MovesPlayed => _board.MovesToSan;
    public EndGameInfo? EndGame => _board.EndGame;
    public int MovesMade => _board.MoveIndex + 1;
    public string? LastMove => _board.MovesToSan.Count > 0 ? _board.MovesToSan[^1] : null;

    public bool IsPlayer1(string userId) => Players[0].UserId == userId;

    public Player GetPlayer(string userId) => Players[GetPlayerIdx(userId)];

    public Player GetOpponent(string userId) => Players[GetOpponentIdx(userId)];

    public int GetPlayerIdx(string userId) => Players[0].UserId == userId ? 0 :
        Players[1].UserId == userId ? 1 :
        throw new ArgumentException("The userId doesn't belong to any of the players in this game.");

    public int GetOpponentIdx(string userId) => GetPlayerIdx(userId) == 0 ? 1 : 0;

    public bool CanMove(string userId)
    {
        bool isPlayer1 = IsPlayer1(userId);

        return (isPlayer1 && IsPlayer1Turn) || (!isPlayer1 && !IsPlayer1Turn);
    }

    public bool Move(string userId, string move, out TimeSpan remainingTime)
    {
        var player = GetPlayer(userId);
        var playerColor = player == Players[0] ? PlayerColors[0] : PlayerColors[1];

        if (playerColor != _board.Turn)
        {
            throw new ArgumentException("It's not your turn.");
        }

        if (TimeControl == TimeControl.Untimed)
        {
            remainingTime = TimeSpan.Zero;
        }
        else
        {
            if (player == Players[0])
            {
                RemainingTimes[0] -= (DateTime.UtcNow - _lastMoveTime);
                remainingTime = RemainingTimes[0];
            }
            else
            {
                RemainingTimes[1] -= (DateTime.UtcNow - _lastMoveTime);
                remainingTime = RemainingTimes[1];
            }

            _lastMoveTime = DateTime.UtcNow;
        }

        try
        {
            return _board.Move(move);
        }
        catch (ChessSanNotFoundException e)
        {
            throw new HubException("Invalid move!");
        }
    }

    public TimeSpan CalculateRemainingTime(int playerIdx)
    {
        if (playerIdx != 0 && playerIdx != 1)
            throw new ArgumentException($"Argument {nameof(playerIdx)} must be 0 or 1.");

        bool thisPlayersTurn = (IsPlayer1Turn && playerIdx == 0) || (!IsPlayer1Turn && playerIdx == 1);
        if (!thisPlayersTurn)
        {
            return RemainingTimes[playerIdx];
        }

        return RemainingTimes[playerIdx] - (DateTime.UtcNow - _lastMoveTime);
    }

    public bool IsPlayer1OutOfTime()
    {
        if (RemainingTimes[0] <= TimeSpan.Zero)
            return true;

        bool isItPlayer1Turn = PlayerColors[0] == _board.Turn;
        if (!isItPlayer1Turn)
            return false;

        var res = DateTime.UtcNow - _lastMoveTime > RemainingTimes[0];
        if (res)
            _board.EndByTimeout(PlayerColors[0]);
        return res;
    }

    public bool IsPlayer2OutOfTime()
    {
        if (RemainingTimes[1] <= TimeSpan.Zero)
            return true;

        bool isItPlayer2Turn = PlayerColors[0] == _board.Turn;
        if (!isItPlayer2Turn)
            return false;


        var res = DateTime.UtcNow - _lastMoveTime > RemainingTimes[1];
        if (res)
            _board.EndByTimeout(PlayerColors[1]);
        return res;
    }

    public void Resign(Player player)
    {
        if (player == Players[0])
            Resign(PlayerColors[0]);
        else if (player == Players[1])
            Resign(PlayerColors[1]);
    }

    public void Resign(PieceColor color)
    {
        _board.Resign(color);
    }

    public void Draw()
    {
        _board.Draw();
    }
}
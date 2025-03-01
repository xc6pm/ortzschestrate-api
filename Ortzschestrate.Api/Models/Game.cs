using System.Numerics;
using Chess;
using Microsoft.AspNetCore.SignalR;

namespace Ortzschestrate.Api.Models;

public class Game
{
    private readonly ChessBoard _board = new();
    private DateTime _lastMoveTime;

    public Game(PendingGame pendingGame, Player player2)
    {
        Player1 = pendingGame.Creator;
        Player2 = player2;
        Player1Color = pendingGame.CreatorColor;
        Player2Color = Player1Color == PieceColor.White ? PieceColor.Black : PieceColor.White;
        StakeEth = pendingGame.StakeEth;
        GameType = pendingGame.GameType;
        Id = Guid.NewGuid();
        StartedTime = _lastMoveTime = DateTime.UtcNow;
        Player1RemainingTime = Player2RemainingTime = pendingGame.GameType.GetTimeSpan();
    }

    public Guid Id { get; }

    public Player Player1 { get; }
    public Player Player2 { get; }
    public PieceColor Player1Color { get; }
    public PieceColor Player2Color { get; }
    public double StakeEth { get; }
    public GameType GameType { get; }
    public DateTime StartedTime { get; }
    public TimeSpan Player1RemainingTime { get; private set; }
    public TimeSpan Player2RemainingTime { get; private set; }

    public string Pgn => _board.ToPgn();
    public EndGameInfo? EndGame => _board.EndGame;
    public int MovesMade => _board.MoveIndex + 1;

    public bool Move(string userId, string move, out TimeSpan remainingTime)
    {
        var player = Player1.UserId == userId ? Player1 :
            Player2.UserId == userId ? Player2 :
            throw new ArgumentException("The userId doesn't belong to any of the players in this game.");
        var playerColor = player == Player1 ? Player1Color : Player2Color;

        if (playerColor != _board.Turn)
        {
            throw new ArgumentException("It's not your turn.");
        }

        if (GameType == GameType.Untimed)
        {
            remainingTime = TimeSpan.Zero;
        }
        else
        {
            if (player == Player1)
            {
                Player1RemainingTime -= (DateTime.UtcNow - _lastMoveTime);
                remainingTime = Player1RemainingTime;
            }
            else
            {
                Player2RemainingTime -= (DateTime.UtcNow - _lastMoveTime);
                remainingTime = Player2RemainingTime;
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

    public bool IsPlayer1OutOfTime()
    {
        if (Player1RemainingTime <= TimeSpan.Zero)
            return true;

        bool isItPlayer1Turn = Player1Color == _board.Turn;
        if (!isItPlayer1Turn)
            return false;

        var res = DateTime.UtcNow - _lastMoveTime > Player1RemainingTime;
        if (res)
            _board.Resign(Player1Color);
        return res;
    }

    public bool IsPlayer2OutOfTime()
    {
        if (Player2RemainingTime <= TimeSpan.Zero)
            return true;

        bool isItPlayer2Turn = Player1Color == _board.Turn;
        if (!isItPlayer2Turn)
            return false;


        var res = DateTime.UtcNow - _lastMoveTime > Player2RemainingTime;
        if (res)
            _board.Resign(Player2Color);
        return res;
    }

    public void Resign(Player player)
    {
        if (player == Player1)
            Resign(Player1Color);
        else if (player == Player2)
            Resign(Player2Color);
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
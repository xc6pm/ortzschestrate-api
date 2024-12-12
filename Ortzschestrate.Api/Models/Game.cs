using Chess;
using Microsoft.AspNetCore.SignalR;

namespace Ortzschestrate.Api.Models;

public class Game
{
    private readonly ChessBoard _board = new();
    private DateTime _lastMoveTime;

    public Game(PendingGame pendingGame, Player player2, string player2ConnectionId)
    {
        Player1 = pendingGame.Creator;
        Player2 = player2;
        Player1ConnectionId = pendingGame.CreatorConnectionId;
        Player2ConnectionId = player2ConnectionId;
        Player1Color = pendingGame.CreatorColor;
        Player2Color = Player1Color == PieceColor.White ? PieceColor.Black : PieceColor.White;
        GameType = pendingGame.GameType;
        Id = Player1ConnectionId + Player2ConnectionId;
        StartedTime = _lastMoveTime = DateTime.UtcNow;
        Player1RemainingTime = Player2RemainingTime = pendingGame.GameType.GetTimeSpan();
    }

    public string Id { get; }

    public Player Player1 { get; }
    public Player Player2 { get; }
    public string Player1ConnectionId { get; }
    public string Player2ConnectionId { get; }
    public PieceColor Player1Color { get; }
    public PieceColor Player2Color { get; }
    public GameType GameType { get; }
    public DateTime StartedTime { get; }
    public TimeSpan Player1RemainingTime { get; private set; }
    public TimeSpan Player2RemainingTime { get; private set; }

    public string Pgn => _board.ToPgn();
    public EndGameInfo? EndGame => _board.EndGame;

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
}
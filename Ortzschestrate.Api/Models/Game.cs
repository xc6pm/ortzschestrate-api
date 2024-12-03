using Chess;
using Microsoft.AspNetCore.SignalR;

namespace Ortzschestrate.Api.Models;

public class Game
{
    private readonly ChessBoard _board = new();

    public Game(PendingGame pendingGame, Player player2, string player2ConnectionId)
    {
        Player1 = pendingGame.Creator;
        Player2 = player2;
        Player1ConnectionId = pendingGame.CreatorConnectionId;
        Player2ConnectionId = player2ConnectionId;
        Player1Color = pendingGame.CreatorColor;
        Player2Color = Player1Color == PieceColor.White ? PieceColor.Black : PieceColor.White;
        Id = Player1ConnectionId + Player2ConnectionId;
    }

    public string Id { get; }

    public Player Player1 { get; }
    public Player Player2 { get; }
    public string Player1ConnectionId { get; }
    public string Player2ConnectionId { get; }
    public PieceColor Player1Color { get; }
    public PieceColor Player2Color { get; }

    public string Pgn => _board.ToPgn();

    public bool Move(string userId, string move)
    {
        var player = Player1.UserId == userId ? Player1 :
            Player2.UserId == userId ? Player2 :
            throw new ArgumentException("The userId doesn't belong to any of the players in this game.");
        var playerColor = player == Player1 ? Player1Color : Player2Color;

        if (playerColor != _board.Turn)
        {
            throw new ArgumentException("It's not your turn.");
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
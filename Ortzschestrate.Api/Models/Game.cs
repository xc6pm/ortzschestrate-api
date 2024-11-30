using Chess;

namespace Ortzschestrate.Api.Models;

public class Game
{
    private readonly ChessBoard _board = new ChessBoard();

    public Game(Player player1, Player player2)
    {
        player1.Color = PieceColor.White;
        player2.Color = PieceColor.Black;
        Player1 = player1;
        Player2 = player2;
    }

    public string Id { get; } = Guid.NewGuid().ToString();

    public Player Player1 { get; }
    public Player Player2 { get; }

    public string Pgn => _board.ToPgn();

    public bool Move(string userId, string move)
    {
        var player = Player1.UserId == userId ? Player1 :
            Player2.UserId == userId ? Player2 :
            throw new ArgumentException("The userId doesn't belong to any of the players in this game.");

        if (player.Color != _board.Turn)
        {
            throw new ArgumentException("It's not that player's turn.");
        }

        return _board.Move(move);
    }
}
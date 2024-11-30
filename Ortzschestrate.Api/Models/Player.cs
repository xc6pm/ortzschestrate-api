using Chess;

namespace Ortzschestrate.Api.Models;

public class Player(string userId, string name)
{
    public string UserId { get; } = userId;
    public string Name { get; } = name;

    public string? GameId { get; set; }
    public PieceColor? Color { get; set; }
}
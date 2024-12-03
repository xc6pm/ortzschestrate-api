using Chess;

namespace Ortzschestrate.Api.Models;

public record PendingGame(Player Creator, string CreatorConnectionId, GameType GameType, PieceColor CreatorColor);
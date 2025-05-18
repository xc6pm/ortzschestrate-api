using Chess;

namespace Ortzschestrate.Api.Models;

public record PendingGame(Player Creator, TimeControl TimeControl, PieceColor CreatorColor, double StakeEth);
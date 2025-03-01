using System.Numerics;
using Chess;

namespace Ortzschestrate.Api.Models;

public record PendingGame(Player Creator, GameType GameType, PieceColor CreatorColor, double StakeEth);
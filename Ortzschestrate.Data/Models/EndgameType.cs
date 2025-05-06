namespace Ortzschestrate.Data.Models;

public enum EndgameType : byte
{
    Checkmate,
    Resigned,
    Timeout,
    Stalemate,
    DrawDeclared,
    InsufficientMaterial,
    FiftyMoveRule,
    Repetition,
}
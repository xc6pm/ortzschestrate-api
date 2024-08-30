import { type Sqr } from "~/models/Square"
import { type Board } from "~/models/Board"
import { findPawnMoves } from "./pawn"
import { findBishopMoves } from "./bishop"
import { findRookMoves } from "./rook"
import { findKnightMoves } from "./knight"

export const movesForPieceOnSqr = (board: Board, sqr: Sqr): Sqr[] => {
  if (!sqr.piece) return []

  switch (sqr.piece?.type) {
    case "pawn":
      return findPawnMoves(board, sqr)
    case "rook":
      return findRookMoves(board, sqr)
    case "knight":
      return findKnightMoves(board, sqr)
    case "bishop":
      return findBishopMoves(board, sqr)
    default:
      break
  }

  return []
}

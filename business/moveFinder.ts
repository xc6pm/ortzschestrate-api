import { SqrLocEnum, type Sqr } from "~/models/Square"
import { type Board } from "~/models/Board"
import { findPawnMoves } from "./pawn"

export const movesForPieceOnSqr = (board: Board, sqr: Sqr): Sqr[] => {
  if (!sqr.piece) return []

  switch (sqr.piece?.type) {
    case "pawn":
      return findPawnMoves(board, sqr)
    default:
      break
  }

  return []
}

import { describe, expect, test } from "vitest"
import { movesForPieceOnSqr } from "./moveFinder"
import { initBoard } from "~/models/Board"


describe("pawn", () => {
  test("pawns can go two steps initially", () => {
    const board = initBoard()

    board.filter(sqr => sqr.piece && sqr.piece.type === "pawn").forEach(sqr => {
      const moves = movesForPieceOnSqr(board, sqr)
      expect(moves).toHaveLength(2)
      // The two moves are on the same col but different rows
      expect(new Set(moves.map(m => m.loc[0]))).toHaveLength(1)
      expect(new Set(moves.map(m => m.loc[1]))).toHaveLength(2)
    })
  })
})

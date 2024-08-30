import { findSqrAt, type Board } from "~/models/Board"
import type { Sqr, SqrLoc } from "~/models/Square"

export const findKnightMoves = (board: Board, sqr: Sqr) => {
  const res: Sqr[] = []

  const addSqrIfAvailable = (x: number, y: number) => {
    const col = sqr.loc.charCodeAt(0)
    const row = parseInt(sqr.loc[1])

    if (col + x < 97 || col + x > 104) return

    if (row + y < 1 || row + y > 8) return

    const targetSqrLoc = String.fromCharCode(col + x) + (row + y).toString()
    const targetSqr = findSqrAt(board, targetSqrLoc as SqrLoc)

    if (targetSqr.piece && targetSqr.piece.color === sqr.piece?.color) return

    res.push(targetSqr)
  }

  addSqrIfAvailable(1, 2)
  addSqrIfAvailable(2, 1)
  addSqrIfAvailable(-1, 2)
  addSqrIfAvailable(-2, 1)
  addSqrIfAvailable(1, -2)
  addSqrIfAvailable(2, -1)
  addSqrIfAvailable(-1, -2)
  addSqrIfAvailable(-2, -1)

  return res
}

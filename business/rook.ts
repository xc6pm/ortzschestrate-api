import { findSqrAt, type Board } from "~/models/Board"
import type { Sqr, SqrLoc } from "~/models/Square"

export const findRookMoves = (board: Board, sqr: Sqr) => {
  const res: Sqr[] = []

  const movesAtDirection = (x: number, y: number) => {
    let col = sqr.loc[0]
    let row = parseInt(sqr.loc[1])
    while (true) {
      const colCode = col.charCodeAt(0) + x
      col = String.fromCharCode(colCode)
      row = row + y
      if (colCode < 97 || colCode > 104 || row < 1 || row > 8) {
        break
      }
      const bottomLeftLoc = (col + row.toString()) as SqrLoc

      const sqrToMove = findSqrAt(board, bottomLeftLoc)
      if (sqrToMove.piece) {
        if (sqrToMove.piece.color !== sqr.piece?.color) {
          res.push(sqrToMove)
        }
        break
      }
      res.push(sqrToMove)
    }
  }

  movesAtDirection(0, 1)
  movesAtDirection(1, 0)
  movesAtDirection(0, -1)
  movesAtDirection(-1, 0)

  return res
}

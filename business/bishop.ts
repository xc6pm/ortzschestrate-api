import { findSqrAt, type Board } from "~/models/Board"
import type { Sqr, SqrLoc } from "~/models/Square"

export const findBishopMoves = (board: Board, sqr: Sqr): Sqr[] => {
  const res: Sqr[] = []
  const diagonalAtDirection = (x: number, y: number) => {
    let col = sqr.loc[0]
    let row = parseInt(sqr.loc[1])
    while (true) {
      const colCode = col.charCodeAt(0) + y
      col = String.fromCharCode(colCode)
      row = row + x
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

  diagonalAtDirection(-1, -1)
  diagonalAtDirection(-1, 1)
  diagonalAtDirection(1, -1)
  diagonalAtDirection(1, 1)

  return res
}

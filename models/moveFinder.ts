import { findSqrAt, type Board, type Game } from "./Board"
import type { Sqr, SqrLoc } from "./Square"

export const findMoves = (board: Board, sqr: Sqr) => {
  const getSqrIfCanMoveTo = (x: number, y: number): Sqr | null => {
    const col = sqr.loc.charCodeAt(0)
    const row = parseInt(sqr.loc[1])

    if (col + x < 97 || col + x > 104) return null

    if (row + y < 1 || row + y > 8) return null

    const targetSqrLoc = String.fromCharCode(col + x) + (row + y).toString()
    const targetSqr = findSqrAt(board, targetSqrLoc as SqrLoc)

    if (targetSqr.piece && targetSqr.piece.color === sqr.piece?.color)
      return null

    return targetSqr
  }

  const getContinuousMovesAtDirection = (x: number, y: number) => {
    const res = []

    while (true) {
      const targetSqr = getSqrIfCanMoveTo(x, y)
      if (!targetSqr) break

      res.push(targetSqr)

      // Guaranteed to be opponent piece
      if (targetSqr.piece) break

      if (x > 0) x++
      else if (x < 0) x--
      if (y > 0) y++
      else if (y < 0) y--
    }
    return res
  }

  const findKnightMoves = (board: Board, sqr: Sqr) => {
    const moveDirections = [
      [1, 2],
      [2, 1],
      [-1, 2],
      [-2, 1],
      [1, -2],
      [2, -1],
      [-1, -2],
      [-2, -1],
    ]

    return moveDirections
      .map((d) => getSqrIfCanMoveTo(d[0], d[1]))
      .filter((s) => s)
  }

  const findRookMoves = (board: Board, sqr: Sqr) => {
    const directions = [
      [0, 1],
      [1, 0],
      [0, -1],
      [-1, 0],
    ]

    return directions
      .map((d) => getContinuousMovesAtDirection(d[0], d[1]))
      .filter((m) => m.length)
      .flat()
  }

  const findBishopMoves = (board: Board, sqr: Sqr): Sqr[] => {
    const directions = [
      [-1, -1],
      [-1, 1],
      [1, -1],
      [1, 1],
    ]
    return directions
      .map((d) => getContinuousMovesAtDirection(d[0], d[1]))
      .filter((m) => m.length)
      .flat()
  }

  const findPawnMoves = () => {
    const pawnForwardMoves = (): Sqr[] => {
      const findSqrForward = (board: Board, sqr: Sqr, sqrsForward: 1 | 2) => {
        const sqrForward = findSqrAt(
          board,
          sqr.loc.replace(
            sqr.loc[1],
            sqr.piece?.color === "white"
              ? (parseInt(sqr.loc[1]) + sqrsForward).toString()
              : (parseInt(sqr.loc[1]) - sqrsForward).toString()
          ) as SqrLoc
        )
        return sqrForward
      }

      const sqrForward = findSqrForward(board, sqr, 1)
      if (sqrForward.piece) {
        return []
      }

      const res = [sqrForward]
      const onInitialRank =
        sqr.piece?.color === "white"
          ? sqr.loc.endsWith("2")
          : sqr.loc.endsWith("7")

      if (onInitialRank) {
        const twoSqrsAhead = findSqrForward(board, sqr, 2)
        if (!twoSqrsAhead.piece) {
          res.push(twoSqrsAhead)
        }
      }

      return res
    }

    const pawnCaptures = (): Sqr[] => {
      const findCaptureOnLeft = () => {
        const pawnIsOnLeftSide = sqr.loc[0] === "a"
        if (pawnIsOnLeftSide) {
          return []
        }

        const sqrLocAtTopLeft =
          String.fromCharCode(sqr.loc.charCodeAt(0) - 1) +
          (
            parseInt(sqr.loc[1]) + (sqr.piece?.color === "white" ? +1 : -1)
          ).toString()
        const sqrOnTopLeft = findSqrAt(board, sqrLocAtTopLeft as SqrLoc)
        if (sqrOnTopLeft.piece) {
          return [sqrOnTopLeft]
        }

        return []
      }
      const findCaptureOnRight = () => {
        const pawnIsOnRightSide = sqr.loc[0] === "h"
        if (pawnIsOnRightSide) {
          return []
        }

        const sqrLocAtTopRight =
          String.fromCharCode(sqr.loc.charCodeAt(0) + 1) +
          (
            parseInt(sqr.loc[1]) + (sqr.piece?.color === "white" ? +1 : -1)
          ).toString()
        const sqrOnTopRight = findSqrAt(board, sqrLocAtTopRight as SqrLoc)
        if (sqrOnTopRight.piece) {
          return [sqrOnTopRight]
        }

        return []
      }

      return findCaptureOnLeft().concat(findCaptureOnRight())
    }

    return [...pawnForwardMoves(), ...pawnCaptures()]
  }

  /// Does not account for checks
  const findKingMoves = () => {
    const directions = [
      [0, 1],
      [1, 1],
      [1, 0],
      [1, -1],
      [0, -1],
      [-1, -1],
      [-1, 0],
      [-1, 1],
    ]

    return directions.map((d) => getSqrIfCanMoveTo(d[0], d[1])).filter((s) => s)
  }

  const findQueenMoves = () => {
    const directions = [
      [0, 1],
      [1, 1],
      [1, 0],
      [1, -1],
      [0, -1],
      [-1, -1],
      [-1, 0],
      [-1, 1],
    ]

    return directions
      .map((d) => getContinuousMovesAtDirection(d[0], d[1]))
      .filter((m) => m.length)
      .flat()
  }

  if (!sqr.piece) return []

  switch (sqr.piece?.type) {
    case "pawn":
      return findPawnMoves()
    case "rook":
      return findRookMoves(board, sqr)
    case "knight":
      return findKnightMoves(board, sqr)
    case "bishop":
      return findBishopMoves(board, sqr)
    case "queen":
      return findQueenMoves()
    case "king":
      return findKingMoves()
    default:
      break
  }

  return []
}

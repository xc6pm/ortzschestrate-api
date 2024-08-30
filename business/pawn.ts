import { findSqrAt, type Board } from "~/models/Board"
import type { Sqr, SqrLoc } from "~/models/Square"

export const findPawnMoves = (board: Board, sqr: Sqr) =>
  pawnForwardMoves(board, sqr).concat(pawnCaptures(board, sqr))

const pawnForwardMoves = (board: Board, sqr: Sqr): Sqr[] => {
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
  const onInitialRank = sqr.piece?.color === "white" ? sqr.loc.endsWith("2") : sqr.loc.endsWith("7")

  if (onInitialRank) {
    const twoSqrsAhead = findSqrForward(board, sqr, 2)
    if (!twoSqrsAhead.piece) {
      res.push(twoSqrsAhead)
    }
  }

  return res
}

const pawnCaptures = (board: Board, sqr: Sqr): Sqr[] => {
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

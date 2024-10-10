import { isNumber, isUpperCase } from "~/utils/stringUtils"
import { Move, type BoardUpdate } from "./BoardUpdate"
import type { Color, PieceType } from "./Piece"
import type { Sqr, SqrLoc } from "./Square"
import type Board from "./Board"
import { isMoveImpossibleDueToPin } from "./checkCalculation"

export function moveToNotation(move: Move) {
  const movedPiece = movedPieceSign(move)
  const checkOrMateSign = move.checkmate ? "#" : move.check ? "+" : ""
  return (
    movedPiece +
    (move.targetSqr.piece ? "x" : "") +
    move.targetSqr.loc +
    checkOrMateSign
  )
}

export function notationToMove(
  notation: string,
  lastMove: BoardUpdate
): {
  startingSqr: SqrLoc
  targetSqr: SqrLoc
  enPassant: boolean
} {
  const color = !lastMove.subjectColor
    ? "white"
    : lastMove.subjectColor === "white"
    ? "black"
    : "white"
  const pawnMoved = !isUpperCase(notation[0])
  if (pawnMoved) {
    return pawnNotationToMove(notation, lastMove.updatedBoard, color)
  }

  return majorPieceNotationToMove(notation, lastMove, color)
}

function movedPieceSign(move: Move) {
  const isCapture = move.targetSqr.piece !== undefined

  switch (move.startingSqr.piece!.type) {
    case "pawn":
      if (isCapture) return move.startingSqr.loc[0]
      return ""
    default:
      let res
      if (move.startingSqr.piece!.type === "knight") res = "N"
      else res = move.startingSqr.piece!.type[0].toUpperCase()

      const sameTypePieces = move.prevState.updatedBoard.filter(
        (s) =>
          s.piece?.type === move.startingSqr.piece?.type &&
          s.piece?.color === move.startingSqr.piece?.color &&
          s.piece !== move.startingSqr.piece
      )
      const canSameTypePieceTargetTheSqr = sameTypePieces.some((s) =>
        move.prevState.updatedBoard
          .findMoves(s)
          .some((m) => m.loc === move.targetSqr.loc)
      )
      const isSameTypePieceOnTheSameFile = sameTypePieces.some(
        (s) => s.loc[0] === move.startingSqr.loc[0]
      )

      if (canSameTypePieceTargetTheSqr) {
        res += move.startingSqr.loc[0]
        if (isSameTypePieceOnTheSameFile) {
          res += move.startingSqr.loc[1]
        }
      }

      return res
  }
}

function pawnNotationToMove(notation: string, board: Board, color: Color) {
  const sqrBehindDirection = color === "white" ? -1 : +1
  const pawnCaptured = notation.includes("x")
  if (pawnCaptured) {
    const pawnFile = notation[0]
    const capturedLoc = notation.slice(2, 4)
    const pawnRank = parseInt(capturedLoc[1]) + sqrBehindDirection
    return {
      startingSqr: (pawnFile + pawnRank.toString()) as SqrLoc,
      targetSqr: capturedLoc as SqrLoc,
      enPassant: !board.findSqrAt(capturedLoc as SqrLoc).piece,
    }
  }

  const targetSqr = notation.slice(0, 2) as SqrLoc
  const oneSqrBehind = board.findSqrAt(
    (targetSqr[0] +
      (parseInt(targetSqr[1]) + sqrBehindDirection).toString()) as SqrLoc
  )
  if (
    oneSqrBehind.piece &&
    oneSqrBehind.piece.type === "pawn" &&
    oneSqrBehind.piece.color === color
  ) {
    return { startingSqr: oneSqrBehind.loc, targetSqr, enPassant: false }
  }

  const twoSqrsBehind = board.findSqrAt(
    (targetSqr[0] +
      (parseInt(targetSqr[1]) + sqrBehindDirection * 2).toString()) as SqrLoc
  )
  if (
    twoSqrsBehind.piece &&
    twoSqrsBehind.piece.type === "pawn" &&
    twoSqrsBehind.piece.color === color
  ) {
    return {
      startingSqr: twoSqrsBehind.loc,
      targetSqr,
      enPassant: false,
    }
  }

  throw new Error(
    `The notation ${notation} doesn't refer to a ${color} pawn on board ${board}`
  )
}

function majorPieceNotationToMove(
  notation: string,
  lastMove: BoardUpdate,
  color: Color
) {
  const board = lastMove.updatedBoard
  const { targetSqr, startingSqrHint } = findTargetSqr(notation)

  const pieceType: PieceType = findPieceType(notation)

  const piecesCapableOfMovingToTargetSqr = board.filter(
    (s) =>
      s.piece &&
      s.piece.color === color &&
      s.piece.type === pieceType &&
      board.findMoves(s).some((sq) => sq.loc === targetSqr)
  )

  if (piecesCapableOfMovingToTargetSqr.length === 0)
    throw new Error(`The move ${notation} is not possible on board ${board}`)

  if (piecesCapableOfMovingToTargetSqr.length === 1) {
    return {
      startingSqr: piecesCapableOfMovingToTargetSqr[0].loc,
      targetSqr,
      enPassant: false,
    }
  }

  return findWhichPieceMakesTheMove(
    startingSqrHint,
    targetSqr,
    piecesCapableOfMovingToTargetSqr,
    lastMove,
    notation
  )
}

function findTargetSqr(notation: string): {
  targetSqr: SqrLoc
  startingSqrHint: string
} {
  const indexOfX = notation.indexOf("x")
  const isCapture = indexOfX !== -1
  let targetSqr: SqrLoc
  let startingSqrHint: string = ""
  if (isCapture) {
    targetSqr = notation.slice(indexOfX + 1, indexOfX + 3) as SqrLoc
    if (!isUpperCase(targetSqr[indexOfX - 1])) {
      startingSqrHint = notation.slice(1, indexOfX)
    }
  } else {
    const checkOrMateSignAtTheEnd =
      notation.at(-1) === "+" || notation.at(-1) === "#"
    let targetSqrStartIndex = checkOrMateSignAtTheEnd
      ? notation.length - 3
      : notation.length - 2
    targetSqr = notation.slice(
      targetSqrStartIndex,
      targetSqrStartIndex + 2
    ) as SqrLoc

    const hintIncluded = !isUpperCase(notation[targetSqrStartIndex - 1])
    if (hintIncluded) {
      startingSqrHint = notation.slice(1, targetSqrStartIndex)
    }
  }
  return { targetSqr, startingSqrHint }
}

function findPieceType(notation: string): PieceType {
  switch (notation[0]) {
    case "K":
      return "king"
    case "Q":
      return "queen"
    case "B":
      return "bishop"
    case "N":
      return "knight"
    case "R":
      return "rook"
    default: // Pawn is handled above.
      throw new Error(`Unexpected piece type in notation: ${notation[0]}`)
  }
}

function findWhichPieceMakesTheMove(
  hint: string,
  targetSqr: SqrLoc,
  allPossiblePieces: Sqr[],
  lastMove: BoardUpdate,
  notation: string
) {
  const board = lastMove.updatedBoard
  if (!hint) {
    // Let's find if some of the pieces can't move due to pin
    const unpinnedPieces = [...allPossiblePieces]
    for (let p of allPossiblePieces) {
      const fakeMove = new Move(p, board.findSqrAt(targetSqr), lastMove)
      const moveImpossibleDueToPin = isMoveImpossibleDueToPin(fakeMove)
      if (moveImpossibleDueToPin) {
        unpinnedPieces.splice(unpinnedPieces.indexOf(p), 1)
      }
    }

    if (unpinnedPieces.length === 1) {
      return {
        startingSqr: unpinnedPieces[0].loc,
        targetSqr,
        enPassant: false,
      }
    }

    throw new Error(
      `Multiple pieces (${allPossiblePieces.join(
        ","
      )}) can move to ${targetSqr} and the file/rank is not specified in the notation ${notation}`
    )
  }

  const isOnlyHintToRank = hint.length === 1 && isNumber(hint)
  if (!isOnlyHintToRank) {
    const fileSpecified = hint[0]
    allPossiblePieces = allPossiblePieces.filter(
      (s) => s.loc[0] === fileSpecified
    )
    if (allPossiblePieces.length === 0) {
      throw new Error(
        `The notation ${notation} doesn't refer to a piece on board ${board}`
      )
    }
    if (allPossiblePieces.length === 1) {
      return {
        startingSqr: allPossiblePieces[0].loc,
        targetSqr,
        enPassant: false,
      }
    }
  }

  const rankSpecified = hint.at(-1)
  return {
    startingSqr: allPossiblePieces.find((s) => s.loc[1] === rankSpecified)!.loc,
    targetSqr,
    enPassant: false,
  }
}

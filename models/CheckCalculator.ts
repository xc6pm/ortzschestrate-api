import { between } from "~/utils/numberUtils"
import { type BoardUpdate, Move } from "./BoardUpdate"
import type { Color } from "./Piece"
import { Sqr, type SqrLoc } from "./Square"
import Board from "./Board"

// Block in the below description refers to block OR CAPTURE:
// Calculates whether the last move puts the opponent king in check or checkmate position.
// First traverses all moves originating from the player who made the last move and whether they
// threaten the king.
// King moves that are
// Re-assesses the available king moves and prunes the captures that are protected by another piece.
// If it's not mate (not taking possible blocks in the next move into account) we return here
// since the check is evadable in the next move.
// Without taking blocks into account, it's a mate at this point. Thus no king move can
// be made
// Then finds whether it's a double check, a double check can only be evaded by a king move
// and since king has no moves we return here - checkmate.
// If it's not double check, calculates whether it can be blocked. See if the block is safe (it doesn't
// open another pieces way toward the king). If the block is possible.

interface CheckState {
  isCheck: boolean
  isMate: boolean
}

export function moveResultsInCheck(move: Move): CheckState {
  const kingColor = move.subjectColor === "white" ? "black" : "white"
  return calcForColor(kingColor, move)
}

export function isMoveImpossibleDueToPin(hypotheticalMove: Move) {
  return calcForColor(hypotheticalMove.subjectColor!, hypotheticalMove).isCheck
}

function calcForColor(kingColor: Color, move: Move): CheckState {
  const king = findKing(kingColor, move)
  const placesKingCanGo = move.updatedBoard.findMoves(king)
  let sqrsNeededForCheckmate = [king, ...placesKingCanGo]
  let isCheck = false
  const checkingPieces = []

  const sqrsControlledByCheckingSide = new Set<SqrLoc>()
  for (let sqr of move.updatedBoard) {
    if (sqr.piece?.color !== kingColor) {
      const pieceMoves = move.updatedBoard.findMoves(sqr)
      pieceMoves.forEach((m) => sqrsControlledByCheckingSide?.add(m.loc))
      if (pieceMoves.includes(king)) {
        isCheck = true
        checkingPieces.push(move.startingSqr)
      }

      sqrsNeededForCheckmate = sqrsNeededForCheckmate.filter(
        (s) => !pieceMoves.includes(s)
      )
    }
  }

  // No need to re-assess king moves for protected piece captures, those pieces are already
  // removed from sqrsNeededForCheckmate.

  const checkmateIfCannotBeBlocked = sqrsNeededForCheckmate.length === 0

  if (checkmateIfCannotBeBlocked) {
    const doubleCheck = checkingPieces.length >= 2
    if (doubleCheck) {
      return { isCheck, isMate: true }
    }

    // const moves = move.updatedBoard.findMoves(sqr2)
    // See if check can be blocked or captured

    const sqrsCheckCanBeBlockedOn = sqrsToBlockCheck(
      checkingPieces[0],
      king,
      move.updatedBoard
    )

    for (let sqr of move.updatedBoard) {
      if (sqr.piece && sqr.piece.color === kingColor) {
        const blockerPiece = sqr
        const moves = move.updatedBoard.findMoves(sqr)
        const blockSqrs = moves.filter((m) =>
          sqrsCheckCanBeBlockedOn.includes(m)
        )
        if (blockSqrs.length) {
          for (let blockSqr of blockSqrs) {
            // Determined by whether the blocking piece was threatend.
            // This is not certain but if this is false certainly the block is safe.
            const blockPossiblyOpensNewWayToKing =
              sqrsControlledByCheckingSide.has(blockSqr.loc)

            if (!blockPossiblyOpensNewWayToKing)
              return { isCheck, isMate: false }

            const tempBoard = new Board(
              move.updatedBoard.map((s) =>
                s === blockSqr
                  ? Sqr.makeCopy(s)
                  : s === blockerPiece
                  ? Sqr.makeCopy(s)
                  : s
              )
            )

            let isBlockEffective = true
            for (let sqr of tempBoard) {
              if (sqr.piece?.color !== kingColor) {
                const pieceMoves = move.updatedBoard.findMoves(sqr)
                if (pieceMoves.includes(king)) {
                  isBlockEffective = false
                  break
                }
              }
            }

            if (isBlockEffective) {
              return {
                isCheck,
                isMate: false,
              }
            }

            return {
              isCheck,
              isMate: true,
            }
          }
        }
      }
    }
  }

  return { isCheck, isMate: false }
}

function sqrsToBlockCheck(checkingPieceSqr: Sqr, kingSqr: Sqr, board: Board) {
  switch (checkingPieceSqr.piece?.type) {
    case "pawn":
    case "knight":
      return [checkingPieceSqr]
    case "rook":
    case "bishop":
    case "queen":
      const res = board.findMoves(checkingPieceSqr).filter(
        (m) =>
          between(
            // Same file
            m.loc.charCodeAt(0),
            checkingPieceSqr.loc.charCodeAt(0),
            kingSqr.loc.charCodeAt(0)
          ) &&
          between(
            // Same rank
            m.loc.charCodeAt(1),
            checkingPieceSqr.loc.charCodeAt(1),
            kingSqr.loc.charCodeAt(1)
          )
      )
      res.push(checkingPieceSqr)
      return res
    default: // Only king remaining, should never happen
      throw new Error("A check cannot originate from a king!")
  }
}

function findKing(kingColor: Color, lastMove: BoardUpdate): Sqr {
  const lastMoveOfKing = (boardUpdate: BoardUpdate) => {
    const reachedInitialBoard = !(boardUpdate instanceof Move)
    if (reachedInitialBoard) {
      return null
    }

    if (
      boardUpdate.startingSqr.piece?.type === "king" &&
      boardUpdate.startingSqr.piece?.color === kingColor
    ) {
      return boardUpdate
    }

    return lastMoveOfKing(boardUpdate.prevState)
  }

  const lastSqrKingMovedTo = lastMoveOfKing(lastMove)
  if (lastSqrKingMovedTo) {
    return lastMove.updatedBoard.findSqrAt(lastSqrKingMovedTo.targetSqr.loc)
  } else if (kingColor === "white") {
    return lastMove.updatedBoard.findSqrAt("e1")
  } else {
    return lastMove.updatedBoard.findSqrAt("e8")
  }
}

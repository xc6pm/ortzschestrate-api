import Board from "./Board"
import type { Color, Piece } from "./Piece"
import { Sqr } from "./Square"

export interface BoardUpdate {
  readonly prevState: BoardUpdate
  readonly updatedBoard: Board
  readonly subjectColor?: Color
  readonly whiteCaptures: ReadonlyArray<Piece>
  readonly blackCaptures: ReadonlyArray<Piece>
}

export class InitialBoard implements BoardUpdate {
  readonly prevState: BoardUpdate = this
  readonly updatedBoard: Board
  readonly subjectColor?: Color | undefined = undefined
  readonly whiteCaptures: ReadonlyArray<Piece> = []
  readonly blackCaptures: ReadonlyArray<Piece> = []

  constructor() {
    this.updatedBoard = Board.setupNew()
  }
}

export class Move implements BoardUpdate {
  public readonly startingSqr: Sqr
  public readonly targetSqr: Sqr
  public readonly prevState: BoardUpdate
  public readonly whiteCaptures: ReadonlyArray<Piece>
  public readonly blackCaptures: ReadonlyArray<Piece>
  public readonly updatedBoard: Board
  public readonly subjectColor?: Color | undefined

  constructor(startingSqr: Sqr, targetSqr: Sqr, prevState: BoardUpdate) {
    this.startingSqr = startingSqr
    this.targetSqr = targetSqr
    this.prevState = prevState
    this.subjectColor = startingSqr.piece?.color
    this.whiteCaptures = prevState.whiteCaptures
    this.blackCaptures = prevState.blackCaptures
    if (this.subjectColor! === "white" && targetSqr.piece) {
      this.whiteCaptures = Object.freeze([...prevState.whiteCaptures, targetSqr.piece!])
    } else if (this.subjectColor! === "black" && targetSqr.piece) {
      this.blackCaptures = Object.freeze([...prevState.blackCaptures, targetSqr.piece!])
    }
    this.updatedBoard = this.calcNewBoard()
  }

  private calcNewBoard() {
    const pieceToBeMoved = this.startingSqr.piece
    return new Board(
      ...this.prevState.updatedBoard.map((s) => {
        if (s.loc === this.targetSqr.loc) {
          const copy = Sqr.makeCopy(s)
          copy.piece = pieceToBeMoved
          return copy
        }
        if (s.loc === this.startingSqr.loc) {
          const copy = Sqr.makeCopy(s)
          copy.piece = undefined
          return copy
        }
        return Sqr.makeCopy(s)
      })
    )
  }

  public toString() {
    const movedPiece = this.movedPieceNotation()
    return movedPiece + (this.targetSqr.piece ? "x" : "") + this.targetSqr.loc
  }

  private movedPieceNotation() {
    const isCapture = this.targetSqr.piece !== undefined

    switch (this.startingSqr.piece!.type) {
      case "pawn":
        if (isCapture) return this.startingSqr.loc[0]
        return ""
      default:
        let res
        if (this.startingSqr.piece!.type === "knight") res = "N"
        else res = this.startingSqr.piece!.type[0].toUpperCase()

        const sameTypePieces = this.prevState.updatedBoard.filter(
          (s) =>
            s.piece?.type === this.startingSqr.piece?.type &&
            s.piece?.color === this.startingSqr.piece?.color &&
            s.piece !== this.startingSqr.piece
        )
        const canSameTypePieceTargetTheSqr = sameTypePieces.some((s) =>
          this.prevState.updatedBoard.findMoves(s).some((m) => m.loc === this.targetSqr.loc)
        )
        const isSameTypePieceOnTheSameFile = sameTypePieces.some(
          (s) => s.loc[0] === this.startingSqr.loc[0]
        )

        if (canSameTypePieceTargetTheSqr) {
          res += this.startingSqr.loc[0]
          if (isSameTypePieceOnTheSameFile) {
            res += this.startingSqr.loc[1]
          }
        }

        return res
    }
  }
}

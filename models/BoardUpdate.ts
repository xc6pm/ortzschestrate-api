import Board from "./Board"
import type { Color } from "./Piece"
import { Sqr } from "./Square"

export interface BoardUpdate {
  readonly prevBoard: Board
  readonly newBoard: Board
  readonly subjectColor?: Color
}

export class InitialBoard implements BoardUpdate {
  readonly prevBoard: Board
  readonly newBoard: Board
  readonly subjectColor?: Color | undefined = undefined

  constructor() {
    this.prevBoard = this.newBoard = Board.setupNew()
  }
}

export class Move implements BoardUpdate {
  public readonly startingSqr: Sqr
  public readonly targetSqr: Sqr
  public readonly prevBoard: Board
  private _newBoard?: Board
  public readonly subjectColor?: Color | undefined

  constructor(startingSqr: Sqr, targetSqr: Sqr, board: Board) {
    this.startingSqr = startingSqr
    this.targetSqr = targetSqr
    this.prevBoard = board
    this.subjectColor = startingSqr.piece?.color
  }

  public get newBoard() {
    if (this._newBoard) return this._newBoard

    const pieceToBeMoved = this.startingSqr.piece
    this._newBoard = new Board(
      ...this.prevBoard.map((s) => {
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
    return this._newBoard
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

        const sameTypePieces = this.prevBoard.filter(
          (s) =>
            s.piece?.type === this.startingSqr.piece?.type &&
            s.piece?.color === this.startingSqr.piece?.color &&
            s.piece !== this.startingSqr.piece
        )
        const canSameTypePieceTargetTheSqr = sameTypePieces.some((s) =>
          this.prevBoard.findMoves(s).some((m) => m.loc === this.targetSqr.loc)
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

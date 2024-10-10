import Board from "./Board"
import { isCheck } from "./checkCalculation"
import { moveToNotation } from "./chessNotation"
import type { Color, Piece } from "./Piece"
import { Sqr } from "./Square"

export interface BoardUpdate {
  readonly prevState: BoardUpdate
  readonly updatedBoard: Board
  readonly subjectColor?: Color
  readonly whiteCaptures: ReadonlyArray<Piece>
  readonly blackCaptures: ReadonlyArray<Piece>
  readonly check: boolean
  readonly checkmate: boolean
}

export class InitialBoard implements BoardUpdate {
  readonly prevState: BoardUpdate = this
  readonly updatedBoard: Board
  readonly subjectColor?: Color | undefined = undefined
  readonly whiteCaptures: ReadonlyArray<Piece> = []
  readonly blackCaptures: ReadonlyArray<Piece> = []
  readonly check: boolean
  readonly checkmate: boolean

  constructor() {
    this.updatedBoard = Board.setupNew()
    this.check = this.checkmate = false
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
  public readonly check
  public readonly checkmate

  constructor(startingSqr: Sqr, targetSqr: Sqr, prevState: BoardUpdate) {
    this.startingSqr = startingSqr
    this.targetSqr = targetSqr
    this.prevState = prevState
    this.subjectColor = startingSqr.piece?.color
    this.whiteCaptures = prevState.whiteCaptures
    this.blackCaptures = prevState.blackCaptures
    if (this.subjectColor! === "white" && targetSqr.piece) {
      this.whiteCaptures = Object.freeze([
        ...prevState.whiteCaptures,
        targetSqr.piece!,
      ])
    } else if (this.subjectColor! === "black" && targetSqr.piece) {
      this.blackCaptures = Object.freeze([
        ...prevState.blackCaptures,
        targetSqr.piece!,
      ])
    }
    this.updatedBoard = this.calcNewBoard()

    const checkState = isCheck(this)
    this.check = checkState.isCheck
    this.checkmate = checkState.isMate
  }

  private calcNewBoard() {
    const pieceToBeMoved = this.startingSqr.piece
    return new Board(
      this.prevState.updatedBoard.map((s) => {
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
    return moveToNotation(this)
  }
}

import { movesForPieceOnSqr } from "~/business/moveFinder"
import type { PieceType } from "./Piece"
import { Sqr, SqrLocEnum, type SqrLoc } from "./Square"

export type Board = Sqr[]

export class Game {
  private _board: Board
  private _boardUpdatedHandlers: ((game: Game) => void)[] = []

  constructor() {
    this._board = initBoard()
  }

  public get board(): Board {
    return this._board
  }

  public onBoardUpdated(handler: (game: Game) => void) {
    this._boardUpdatedHandlers.push(handler)
  }

  public findMoves(sqr: Sqr): Sqr[] {
    return movesForPieceOnSqr(this._board, sqr)
  }

  /// Does not perform validation. Given moves must be validated before using findMoves.
  public movePiece(currentSqr: Sqr, targetSqr: Sqr) {
    targetSqr.piece = currentSqr.piece
    currentSqr.piece = undefined
    this.notifyBoardUpdated()
  }

  private notifyBoardUpdated() {
    for (let handler of this._boardUpdatedHandlers) {
      handler(this)
    }
  }
}

export const initBoard = (): Board => {
  const sqrs = []
  for (let i = 0; i < 64; i++) {
    const sqr = new Sqr(SqrLocEnum[i] as SqrLoc)
    sqrs.push(sqr)
  }

  const backRank: PieceType[] = [
    "rook",
    "knight",
    "bishop",
    "queen",
    "king",
    "bishop",
    "knight",
    "rook",
  ]
  for (let i = 0; i < 16; i++) {
    sqrs[i].piece = {
      id: Date.now() + Math.random(),
      color: "white",
      type: i < 8 ? backRank[i] : "pawn",
    }
  }

  for (let i = sqrs.length - 1; i > sqrs.length - 17; i--) {
    sqrs[i].piece = {
      id: Date.now() + Math.random(),
      color: "black",
      type: i > sqrs.length - 9 ? backRank[i - 56] : "pawn",
    }
  }

  return sqrs
}

export const findSqrAt = (board: Board, sqrLoc: SqrLoc) => {
  return board[SqrLocEnum[sqrLoc]]
}

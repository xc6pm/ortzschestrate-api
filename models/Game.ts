import { generateId } from "~/utils/randomGenerator"
import { initBoard, type Board } from "./Board"
import { findMoves } from "./moveFinder"
import type { Sqr } from "./Square"

export class Game {
  private _board: Board
  private _boardUpdatedHandlers: ((game: Game) => void)[] = []
  readonly gameId: number

  constructor() {
    this._board = initBoard()
    this.gameId = generateId()
  }

  public get board(): Board {
    return this._board
  }

  public onBoardUpdated(handler: (game: Game) => void) {
    this._boardUpdatedHandlers.push(handler)
  }

  public findMoves(sqr: Sqr): Sqr[] {
    return findMoves(this._board, sqr)
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

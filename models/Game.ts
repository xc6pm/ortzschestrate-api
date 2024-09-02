import { generateId } from "~/utils/randomGenerator"
import { initBoard, type Board } from "./Board"
import { findMoves } from "./moveFinder"
import { Sqr } from "./Square"

export class Game {
  private _boardStack: Board[] = []
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
    const pieceToBeMoved = currentSqr.piece
    this._boardStack.push(this._board.map(s => {
      if (s.loc === targetSqr.loc) {
        const copy = Sqr.makeCopy(s)
        copy.piece = pieceToBeMoved
        return copy
      }
      if (s.loc === currentSqr.loc) {
        const copy = Sqr.makeCopy(s)
        copy.piece = undefined
        return copy
      }
      return Sqr.makeCopy(s)
    }))
    this._board = this._boardStack[this._boardStack.length - 1]
    
    this.notifyBoardUpdated()
  }

  private notifyBoardUpdated() {
    for (let handler of this._boardUpdatedHandlers) {
      handler(this)
    }
  }
}

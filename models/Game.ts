import { generateId } from "~/utils/randomGenerator"
import Board from "./Board"
import { Sqr } from "./Square"
import { InitialBoard, Move, type BoardUpdate } from "./BoardUpdate"

export class Game {
  private _moveHistory: BoardUpdate[] = []
  private _board: Board
  private _boardUpdatedHandlers: ((game: Game) => void)[] = []
  readonly gameId: number

  constructor() {
    this._moveHistory.push(new InitialBoard())
    this._board = this._moveHistory[0].updatedBoard
    this.gameId = generateId()
  }

  public get board(): Board {
    return this._board
  }
  public get moveHistory(): BoardUpdate[] {
    return this._moveHistory
  }

  public onBoardUpdated(handler: (game: Game) => void) {
    this._boardUpdatedHandlers.push(handler)
  }

  public findMoves(sqr: Sqr) {
    return this._board.findMoves(sqr)
  }

  /// Does not perform validation. Given moves must be validated before using findMoves.
  public movePiece(currentSqr: Sqr, targetSqr: Sqr) {
    if (!currentSqr.piece)
      throw new Error(`There's no piece to be moved on ${currentSqr.loc}.`)

    if (
      this._moveHistory[this._moveHistory.length - 1].subjectColor ===
      currentSqr.piece?.color
    )
      return

    this._moveHistory.push(new Move(currentSqr, targetSqr, this.moveHistory[this.moveHistory.length - 1]))
    this._board = this._moveHistory[this._moveHistory.length - 1].updatedBoard

    this.notifyBoardUpdated()
  }

  private notifyBoardUpdated() {
    for (let handler of this._boardUpdatedHandlers) {
      handler(this)
    }
  }
}

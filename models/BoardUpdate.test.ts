import { beforeEach, describe, expect, test } from "vitest"
import { Move } from "./BoardUpdate"
import { Game } from "./Game"

let game: Game
let board = () => game.board

beforeEach(() => {
  game = new Game()
})

describe("chess notation", () => {
  describe("pawn", () => {
    test("pawn to e4", () => {
      const e2 = board().findSqrAt("e2")
      const e4 = board().findSqrAt("e4")

      const move = new Move(e2, e4, board())

      expect(move.toString()).toBe("e4")
    })
  })

  describe("knight", () => {
    test("b1 first move", () => {
      const b1 = board().findSqrAt("b1")
      const c3 = board().findSqrAt("c3")

      const move = new Move(b1, c3, board())
      expect(move.toString()).toBe("Nc3")
    })

    test("two knights can move to same sqr", () => {
      const d2 = board().findSqrAt("d2")
      game.movePiece(d2, board().findMoves(d2)[1])

      const g1 = board().findSqrAt("g1")
      game.movePiece(g1, board().findSqrAt("f3"))

      const b2KnightMove = new Move(
        board().findSqrAt("b1"),
        board().findSqrAt("d2"),
        board()
      )
      expect(b2KnightMove.toString()).toBe("Nbd2")

      const f3KnightMove = new Move(
        board().findSqrAt("f3"),
        board().findSqrAt("d2"),
        board()
      )
      expect(f3KnightMove.toString()).toBe("Nfd2")
    })
  })
})

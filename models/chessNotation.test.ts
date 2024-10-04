import { beforeEach, describe, expect, test } from "vitest"
import { GameProcessor } from "./GameProcessor"
import { parseGames } from "@mliebelt/pgn-parser"
import { Move } from "./BoardUpdate"
import { readFile } from "node:fs/promises"
import { notationToMove } from "./chessNotation"
import type { Color } from "./Piece"

describe("moveToNotation", () => {
  let gameProcessor: GameProcessor
  let board = () => gameProcessor.board
  let lastBoardUpdate = () =>
    gameProcessor.moveHistory[gameProcessor.moveHistory.length - 1]

  beforeEach(() => {
    gameProcessor = new GameProcessor()
  })

  describe("pawn", () => {
    test("pawn to e4", () => {
      const e2 = board().findSqrAt("e2")
      const e4 = board().findSqrAt("e4")

      const move = new Move(e2, e4, lastBoardUpdate())

      expect(move.toString()).toBe("e4")
    })
  })

  describe("knight", () => {
    test("b1 first move", () => {
      const b1 = board().findSqrAt("b1")
      const c3 = board().findSqrAt("c3")

      const move = new Move(b1, c3, lastBoardUpdate())
      expect(move.toString()).toBe("Nc3")
    })

    test("two knights can move to same sqr", () => {
      const d2 = board().findSqrAt("d2")
      gameProcessor.movePiece(d2, board().findMoves(d2)[1])

      // Black move
      gameProcessor.movePiece(
        board().findSqrAt("d7"),
        board().findMovesAt("d7")[1]
      )

      const g1 = board().findSqrAt("g1")
      gameProcessor.movePiece(g1, board().findSqrAt("f3"))

      const b2KnightMove = new Move(
        board().findSqrAt("b1"),
        board().findSqrAt("d2"),
        lastBoardUpdate()
      )
      expect(b2KnightMove.toString()).toBe("Nbd2")

      const f3KnightMove = new Move(
        board().findSqrAt("f3"),
        board().findSqrAt("d2"),
        lastBoardUpdate()
      )
      expect(f3KnightMove.toString()).toBe("Nfd2")
    })
  })
})

describe("notationToMove", () => {
  test("Against the twic1559 db", async () => {
    const str = await readFile("./twic1559.pgn")
    const games = parseGames(str.toString())

    let i = 0
    // Just see that it doesn't run into errors.
    for (let game of games) {
      i++
      const gameProcessor = new GameProcessor()
      let color: Color = "white"
      let j = 0
      for (let pgnMove of game.moves) {
        j++
        /// Castling not supported yet.
        /// ToDo: Remove this after castling is implemented.
        if (pgnMove.notation.notation.toUpperCase().startsWith("O")) {
          break
        }

        /// Promotions not supported yet.
        /// ToDo: Remove this after promotion is implemented.
        if (pgnMove.notation.notation.includes("=")) {
          break
        }

        if (i === 397 && j === 86) {
          console.log("here")
        }
        const move = notationToMove(
          pgnMove.notation.notation,
          gameProcessor.moveHistory[gameProcessor.moveHistory.length - 1]
        )

        // En Passant not implemented yet.
        // ToDo: Remove this after en passant is implemented.
        if (move.enPassant) break

        gameProcessor.movePiece(
          gameProcessor.board.findSqrAt(move.startingSqr),
          gameProcessor.board.findSqrAt(move.targetSqr)
        )
        color = color === "white" ? "black" : "white"
      }
    }
  })
})

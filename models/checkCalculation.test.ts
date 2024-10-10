import { beforeEach, describe, expect, test } from "vitest"
import { GameProcessor } from "./GameProcessor"
import { parseGames } from "@mliebelt/pgn-parser"
import { readFile } from "node:fs/promises"

let game: GameProcessor
beforeEach(() => {
  game = new GameProcessor()
})

describe("Simple scenarios", () => {
  test("Fools' mate", () => {
    expect(game.moveHistory.at(-1)?.check).toBeFalsy()
    expect(game.moveHistory.at(-1)?.checkmate).toBeFalsy()

    game.movePiece(game.board.findSqrAt("g1"), game.board.findSqrAt("g4"))
    game.movePiece(game.board.findSqrAt("e7"), game.board.findSqrAt("e6"))
    game.movePiece(game.board.findSqrAt("f2"), game.board.findSqrAt("f3"))
    expect(game.moveHistory.at(-1)?.checkmate).toBeFalsy()
    game.movePiece(game.board.findSqrAt("d8"), game.board.findSqrAt("h4"))
    expect(game.moveHistory.at(-1)?.checkmate).toBeTruthy()
  })
})

// test("Test against the week in chess 1559 games", async () => {
//   const str = await readFile("./twic1559.pgn")
//   const games = parseGames(str.toString()).filter((g) =>
//     g.moves.at(-1)?.notation.notation.endsWith("#")
//   )
  

//   for (let game of games) {
//     const gameProcessor = new GameProcessor()
//     for (let move of game.moves) {
//       gameProcessor.movePiece(move.notation.)
//     }
//   }
// })

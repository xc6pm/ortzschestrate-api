import { beforeEach, describe, expect, test } from "vitest"
import { findSqrAt, initBoard, type Board } from "./Board"
import { findMoves } from "./moveFinder"

let board: Board
beforeEach(() => {
  board = initBoard()
})

describe("pawn", () => {
  test("pawns can go two steps initially", () => {
    board
      .filter((sqr) => sqr.piece && sqr.piece.type === "pawn")
      .forEach((sqr) => {
        const moves = findMoves(board, sqr)
        expect(moves).toHaveLength(2)
        // The two moves are on the same col but different rows
        expect(new Set(moves.map((m) => m!.loc[0]))).toHaveLength(1)
        expect(new Set(moves.map((m) => m!.loc[1]))).toHaveLength(2)
      })
  })
})

describe("rook", () => {
  test("a1 rook initial moves", () => {
    const rookSqr = board[0]
    let rookMoves = findMoves(board, rookSqr)
    expect(rookMoves).toHaveLength(0)

    const blockingPawnSqr = findSqrAt(board, "a2")
    const blockingPawnMoves = findMoves(board, blockingPawnSqr)
    blockingPawnMoves[1]!.piece = blockingPawnSqr.piece
    blockingPawnSqr.piece = undefined

    rookMoves = findMoves(board, rookSqr)
    expect(rookMoves).toHaveLength(2)
  })

  test("rook at the middle", () => {
    const e5 = findSqrAt(board, "e5")

    e5.piece = board[0].piece
    board[0].piece = undefined

    const moves = findMoves(board, e5)

    expect(moves).toHaveLength(11)
  })
})

describe("bishop", () => {
  test("c1 bishop initial moves", () => {
    const bishopSqr = board[2]
    let bishopMoves = findMoves(board, bishopSqr)
    expect(bishopMoves).toHaveLength(0)

    const blockingPawnSqr = findSqrAt(board, "d2")
    const blockingPawnMoves = findMoves(board, blockingPawnSqr)
    blockingPawnMoves[0]!.piece = blockingPawnSqr.piece
    blockingPawnSqr.piece = undefined

    bishopMoves = findMoves(board, bishopSqr)
    expect(bishopMoves).toHaveLength(5)
  })

  test("bishop at the middle", () => {
    const e5 = findSqrAt(board, "e5")

    e5.piece = board[2].piece
    board[2].piece = undefined

    const moves = findMoves(board, e5)

    expect(moves).toHaveLength(8)
  })
})

describe("knight", () => {
  test("b1 knight initial moves", () => {
    const knightSqr = board[1]
    let knightMoves = findMoves(board, knightSqr)
    expect(knightMoves).toHaveLength(2)

    const blockingPawnSqr = findSqrAt(board, "d2")
    const blockingPawnMoves = findMoves(board, blockingPawnSqr)
    blockingPawnMoves[1]!.piece = blockingPawnSqr.piece
    blockingPawnSqr.piece = undefined

    knightMoves = findMoves(board, knightSqr)
    expect(knightMoves).toHaveLength(3)
  })

  test("knight at the middle", () => {
    const e2 = findSqrAt(board, "e2")
    findMoves(board, e2)[0]!.piece = e2.piece
    e2.piece = undefined

    const b1 = board[1]
    const d5 = findSqrAt(board, "d5")
    d5.piece = b1.piece
    b1.piece = undefined

    const moves = findMoves(board, d5)

    expect(moves).toHaveLength(7)
  })
})


describe("queen", () => {
  test("white queen initial moves", () => {
    const queenSqr = board[3]
    let queenMoves = findMoves(board, queenSqr)
    expect(queenMoves).toHaveLength(0)

    const blockingPawnSqr = findSqrAt(board, "e2")
    const blockingPawnMoves = findMoves(board, blockingPawnSqr)
    blockingPawnMoves[1]!.piece = blockingPawnSqr.piece
    blockingPawnSqr.piece = undefined

    queenMoves = findMoves(board, queenSqr)
    expect(queenMoves).toHaveLength(4)
  })

  test("queen at the middle", () => {
    const e2 = findSqrAt(board, "e2")
    findMoves(board, e2)[0]!.piece = e2.piece
    e2.piece = undefined

    const d1 = board[3]
    const e4 = findSqrAt(board, "e4")
    e4.piece = d1.piece
    d1.piece = undefined

    const moves = findMoves(board, e4)

    expect(moves).toHaveLength(18)
  })
})

describe("king", () => {
  test("white king initial moves", () => {
    const kingSqr = board[4]
    let kingMoves = findMoves(board, kingSqr)
    expect(kingMoves).toHaveLength(0)

    const blockingPawnSqr = findSqrAt(board, "e2")
    const blockingPawnMoves = findMoves(board, blockingPawnSqr)
    blockingPawnMoves[1]!.piece = blockingPawnSqr.piece
    blockingPawnSqr.piece = undefined

    kingMoves = findMoves(board, kingSqr)
    expect(kingMoves).toHaveLength(1)
  })

  test("king at the middle", () => {
    const e2 = findSqrAt(board, "e2")
    findMoves(board, e2)[0]!.piece = e2.piece
    e2.piece = undefined

    const e1 = board[4]
    const e4 = findSqrAt(board, "e4")
    e4.piece = e1.piece
    e1.piece = undefined

    const moves = findMoves(board, e4)

    expect(moves).toHaveLength(7)
  })
})
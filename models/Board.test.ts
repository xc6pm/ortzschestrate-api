import { expect, test } from "vitest"
import Board from "./Board"
import type { PieceType } from "./Piece"

test("pieces are placed correctly", () => {
  const board = Board.setupNew()

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

  expect(board.length).toEqual(64)

  for (let i = 0; i < 8; i++) {
    expect(board[i].piece?.type).toBe(backRank[i])
  }

  for (let i = 8; i < 16; i++) {
    expect(board[i].piece?.type).toBe("pawn")
  }

  for (let i = 8 * 2; i < 8 * 6; i++) {
    expect(board[i].piece).toBeFalsy()
  }

  for (let i = 8 * 6; i < 8 * 7; i++) {
    expect(board[i].piece?.type).toBe("pawn")
  }

  for (let i = 8 * 7; i < 8 * 8; i++) {
    expect(board[i].piece?.type).toBe(backRank[i - 8 * 7])
  }
})

import { generateId } from "~/utils/randomGenerator"
import type { PieceType } from "./Piece"
import { Sqr, SqrLocEnum, type SqrLoc } from "./Square"

export type Board = Sqr[]

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
      id: generateId(),
      color: "white",
      type: i < 8 ? backRank[i] : "pawn",
    }
  }

  for (let i = sqrs.length - 1; i > sqrs.length - 17; i--) {
    sqrs[i].piece = {
      id: generateId(),
      color: "black",
      type: i > sqrs.length - 9 ? backRank[i - 56] : "pawn",
    }
  }

  return sqrs
}

export const findSqrAt = (board: Board, sqrLoc: SqrLoc) => {
  return board[SqrLocEnum[sqrLoc]]
}

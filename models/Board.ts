import { generateId } from "~/utils/randomGenerator"
import type { PieceType } from "./Piece"
import { Sqr, SqrLocEnum, type SqrLoc } from "./Square"
import { findMoves } from "./moveFinder"

export default class Board extends Array<Sqr> {
  constructor(items: Array<Sqr> | number) {
    if (items instanceof Array) super(...items)
    else super(items)
    Object.setPrototypeOf(this, Board.prototype)
  }

  public findSqrAt(sqrLoc: SqrLoc) {
    return this[SqrLocEnum[sqrLoc]]
  }

  public findMoves(sqr: Sqr) {
    return findMoves(this, sqr)
  }

  public findMovesAt(sqrLoc: SqrLoc) {
    const sqr = this.findSqrAt(sqrLoc)
    return this.findMoves(sqr)
  }

  public override toString() {
    return `[${this.join(", ")}]`
  }

  public static setupNew(): Board {
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

    return new Board(sqrs)
  }
}

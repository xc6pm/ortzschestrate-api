export type Color = "white" | "black"

export type PieceType = "king" | "queen" | "bishop" | "knight" | "rook" | "pawn"

export type Piece = {
  id: number
  color: Color
  type: PieceType
}

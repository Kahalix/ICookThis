export type UnitType = 'Mass' | 'Volume' | 'Piece'

export interface UnitResponse {
  id: number
  symbol: string
  type: UnitType
}

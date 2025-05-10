export interface RecipeIngredientResponse {
  id: number
  recipeId: number
  ingredient: { id: number; name: string }
  qty: number
  unit: { id: number; symbol: string; type: 'Mass' | 'Volume' | 'Piece' }
}

export interface RecipeIngredientRequest {
  ingredientId: number
  qty: number
  unitId: number
}

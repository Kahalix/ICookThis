export interface StepIngredientResponse {
  id: number
  instructionStepId: number
  ingredient: { id: number; name: string }
  fraction: number
}

export interface StepIngredientRequest {
  ingredientId: number
  fraction: number
}

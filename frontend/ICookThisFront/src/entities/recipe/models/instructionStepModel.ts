import type { StepIngredientResponse, StepIngredientRequest } from './stepIngredientModel'

export interface NewInstructionStepRequest {
  stepOrder: number
  templateText: string
  imageFile?: File
  stepIngredients?: StepIngredientRequest[]
}

export interface UpdateInstructionStepRequest {
  id: number
  stepOrder: number
  templateText: string
  image?: string
  imageFile?: File
  removeImage?: boolean
  stepIngredients?: StepIngredientRequest[]
}

export interface InstructionStepResponse {
  id: number
  recipeId: number
  stepOrder: number
  text: string
  image: string
  stepIngredients: StepIngredientResponse[]
}

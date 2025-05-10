import type { StepIngredientResponse, StepIngredientRequest } from './stepIngredientModel'

// do tworzenia nowych kroków
export interface NewInstructionStepRequest {
  stepOrder: number
  templateText: string
  imageFile?: File
  stepIngredients?: StepIngredientRequest[]
}

// do aktualizacji istniejących
export interface UpdateInstructionStepRequest {
  id: number
  stepOrder: number
  templateText: string
  image?: string // aktualny URL
  imageFile?: File // nowy upload
  removeImage?: boolean // jeśli chcemy usunąć istniejący
  stepIngredients?: StepIngredientRequest[]
}

// Response się nie zmienia:
export interface InstructionStepResponse {
  id: number
  recipeId: number
  stepOrder: number
  text: string
  image: string
  stepIngredients: StepIngredientResponse[]
}

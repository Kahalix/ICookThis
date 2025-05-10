import type {
  NewInstructionStepRequest,
  UpdateInstructionStepRequest,
  InstructionStepResponse,
} from './instructionStepModel'
import type { RecipeIngredientResponse, RecipeIngredientRequest } from './recipeIngredientModel'

// requesty
export interface NewRecipeRequest {
  name: string
  defaultQty: number
  defaultUnitId: number
  dishType: 'Appetizer' | 'MainCourse' | 'Dessert' | 'Snack' | 'Beverage'
  description: string
  imageFile?: File
  ingredients: RecipeIngredientRequest[]
  steps: NewInstructionStepRequest[]
}

export interface UpdateRecipeRequest {
  name: string
  defaultQty?: number
  defaultUnitId?: number
  dishType?: 'Appetizer' | 'MainCourse' | 'Dessert' | 'Snack' | 'Beverage'
  description: string
  imageFile?: File
  removeImage?: boolean
  ingredients: RecipeIngredientRequest[]
  steps: UpdateInstructionStepRequest[]
}

export interface RecipeResponse {
  id: number
  name: string
  defaultQty: number
  defaultUnit: { id: number; symbol: string; type: 'Mass' | 'Volume' | 'Piece' }
  dishType: 'Appetizer' | 'MainCourse' | 'Dessert' | 'Snack' | 'Beverage'
  description: string
  image: string
  avgRating: number | null
  avgDifficulty: number | null
  recommendPercentage: number | null
  avgPreparationTimeMinutes: number | null
  ingredients: RecipeIngredientResponse[]
  steps: InstructionStepResponse[]
}

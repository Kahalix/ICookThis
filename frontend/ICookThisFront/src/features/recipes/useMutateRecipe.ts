import { ref } from 'vue'
import type {
  RecipeResponse,
  NewRecipeRequest,
  UpdateRecipeRequest,
} from '@/entities/recipe/models/recipeModel'
import { createRecipe, updateRecipe, deleteRecipe } from '@/entities/recipe/services/recipeService'

export function useCreateRecipe() {
  const loading = ref(false)
  const error = ref<Error | null>(null)
  const result = ref<RecipeResponse | null>(null)

  async function exec(data: NewRecipeRequest & { imageFile?: File }) {
    loading.value = true
    error.value = null
    try {
      result.value = await createRecipe(data)
    } catch (e: unknown) {
      error.value = e instanceof Error ? e : new Error(String(e))
    } finally {
      loading.value = false
    }
  }

  return { loading, error, result, exec }
}

export function useUpdateRecipe() {
  const loading = ref(false)
  const error = ref<Error | null>(null)
  const result = ref<RecipeResponse | null>(null)

  async function exec(id: number, data: UpdateRecipeRequest & { imageFile?: File }) {
    loading.value = true
    error.value = null
    try {
      result.value = await updateRecipe(id, data)
    } catch (e: unknown) {
      error.value = e instanceof Error ? e : new Error(String(e))
    } finally {
      loading.value = false
    }
  }

  return { loading, error, result, exec }
}

export function useDeleteRecipe() {
  const loading = ref(false)
  const error = ref<Error | null>(null)
  const success = ref(false)

  async function exec(id: number) {
    loading.value = true
    error.value = null
    success.value = false
    try {
      await deleteRecipe(id)
      success.value = true
    } catch (e: unknown) {
      error.value = e instanceof Error ? e : new Error(String(e))
    } finally {
      loading.value = false
    }
  }

  return { loading, error, success, exec }
}

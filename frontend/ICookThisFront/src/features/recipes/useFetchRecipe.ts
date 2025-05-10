import { ref } from 'vue'
import type { RecipeResponse } from '@/entities/recipe/models/recipeModel'
import { fetchRecipe } from '@/entities/recipe/services/recipeService'

export function useFetchRecipe() {
  const loading = ref(false)
  const error = ref<Error | null>(null)
  const data = ref<RecipeResponse | null>(null)

  async function load(id: number, scale?: number) {
    loading.value = true
    error.value = null
    try {
      data.value = await fetchRecipe(id, scale)
    } catch (e: unknown) {
      if (e instanceof Error) error.value = e
      else error.value = new Error(String(e))
    } finally {
      loading.value = false
    }
  }

  return { loading, error, data, load }
}

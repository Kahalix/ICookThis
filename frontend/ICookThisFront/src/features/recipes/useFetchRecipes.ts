import { ref } from 'vue'
import type { RecipeResponse } from '@/entities/recipe/models/recipeModel'
import type { PagedResult } from '@/shared/lib/types'
import { fetchRecipes } from '@/entities/recipe/services/recipeService'

export function useFetchRecipes() {
  const loading = ref(false)
  const error = ref<Error | null>(null)
  const data = ref<PagedResult<RecipeResponse>>({
    page: 1,
    pageSize: 10,
    totalCount: 0,
    totalPages: 0,
    items: [],
  })

  async function load(
    page = 1,
    pageSize = 10,
    search = '',
    sortBy: 'Name' | 'AvgRating' | 'AvgPreparationTime' = 'Name',
    sortOrder: 'Asc' | 'Desc' = 'Asc',
    dishType?: string,
  ) {
    loading.value = true
    error.value = null
    try {
      data.value = await fetchRecipes(page, pageSize, search, sortBy, sortOrder, dishType)
    } catch (e: unknown) {
      if (e instanceof Error) error.value = e
      else error.value = new Error(String(e))
    } finally {
      loading.value = false
    }
  }

  return { loading, error, data, load }
}

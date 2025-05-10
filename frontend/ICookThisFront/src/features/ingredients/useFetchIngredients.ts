import { ref } from 'vue'
import type { IngredientResponse } from '@/entities/ingredient/models/ingredientModel'
import type { PagedResult } from '@/shared/lib/types'
import { fetchIngredients } from '@/entities/ingredient/services/ingredientService'

export function useFetchIngredients() {
  const loading = ref(false)
  const error = ref<Error | null>(null)
  const data = ref<PagedResult<IngredientResponse>>({
    page: 1,
    pageSize: 10,
    totalCount: 0,
    totalPages: 0,
    items: [],
  })

  async function load(page = 1, pageSize = 10, search = '') {
    loading.value = true
    error.value = null
    try {
      data.value = await fetchIngredients(page, pageSize, search)
    } catch (e: unknown) {
      error.value = e instanceof Error ? e : new Error(String(e))
    } finally {
      loading.value = false
    }
  }

  return { loading, error, data, load }
}

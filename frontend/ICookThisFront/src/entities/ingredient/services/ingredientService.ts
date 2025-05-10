import { api } from '@shared/lib/api'
import type { PagedResult } from '@/shared/lib/types'
import type { IngredientResponse } from '../models/ingredientModel'
import type { AxiosResponse } from 'axios'

/**
 * Pobiera paginowaną listę składników.
 */
export function fetchIngredients(
  page = 1,
  pageSize = 10,
  search = '',
): Promise<PagedResult<IngredientResponse>> {
  const params: Record<string, unknown> = { page, pageSize, search }
  return api
    .get<PagedResult<IngredientResponse>>('/ingredients', { params })
    .then((r: AxiosResponse<PagedResult<IngredientResponse>>) => r.data)
}

// src/entities/recipe/services/recipeService.ts

import { api } from '@shared/lib/api'
import type { PagedResult } from '@/shared/lib/types'
import type { RecipeResponse, NewRecipeRequest, UpdateRecipeRequest } from '../models/recipeModel' // lub '..' dzięki index.ts
import type { AxiosResponse } from 'axios'

// formatujemy liczby na przecinki
function formatNumberForApi(value: number): string {
  // zamieniamy kropkę na przecinek
  return value.toString().replace('.', ',')
}

/**
 * Pobiera paginowaną listę przepisów.
 */
export function fetchRecipes(
  page = 1,
  pageSize = 10,
  search = '',
  sortBy: 'Name' | 'AvgRating' | 'AvgPreparationTime' = 'Name',
  sortOrder: 'Asc' | 'Desc' = 'Asc',
  dishType?: string,
): Promise<PagedResult<RecipeResponse>> {
  const params: Record<string, unknown> = { page, pageSize, search, sortBy, sortOrder }
  if (dishType) params.dishType = dishType

  return api
    .get<PagedResult<RecipeResponse>>('/recipes', { params })
    .then((r: AxiosResponse<PagedResult<RecipeResponse>>) => r.data)
}

/**
 * Pobiera pojedynczy przepis, opcjonalnie skalując ilości.
 */
export function fetchRecipe(id: number, scale?: number): Promise<RecipeResponse> {
  const params = scale != null ? { scale } : {}
  return api
    .get<RecipeResponse>(`/recipes/${id}`, { params })
    .then((r: AxiosResponse<RecipeResponse>) => r.data)
}

/**
 * Tworzy nowy przepis.
 * @param request Obiekt NewRecipeRequest (z opcjonalnym imageFile)
 */
export function createRecipe(
  request: NewRecipeRequest & { imageFile?: File },
): Promise<RecipeResponse> {
  const form = new FormData()
  form.append('Name', request.name)
  form.append('DefaultQty', formatNumberForApi(request.defaultQty))
  form.append('DefaultUnitId', String(request.defaultUnitId))
  form.append('DishType', request.dishType)
  form.append('Description', request.description)
  if (request.imageFile) form.append('ImageFile', request.imageFile)

  // składniki
  request.ingredients.forEach((ing, i) => {
    form.append(`Ingredients[${i}].IngredientId`, String(ing.ingredientId))
    form.append(`Ingredients[${i}].Qty`, formatNumberForApi(ing.qty))
    form.append(`Ingredients[${i}].UnitId`, String(ing.unitId))
  })

  // kroki – wszystkie jako NewInstructionStepRequest
  request.steps.forEach((st, i) => {
    form.append(`Steps[${i}].StepOrder`, String(st.stepOrder))
    form.append(`Steps[${i}].TemplateText`, st.templateText)
    if (st.imageFile) form.append(`Steps[${i}].ImageFile`, st.imageFile)
    ;(st.stepIngredients ?? []).forEach((psi, j) => {
      form.append(`Steps[${i}].StepIngredients[${j}].IngredientId`, String(psi.ingredientId))
      form.append(`Steps[${i}].StepIngredients[${j}].Fraction`, formatNumberForApi(psi.fraction))
    })
  })

  return api
    .post<RecipeResponse, AxiosResponse<RecipeResponse>, FormData>('/recipes', form, {
      headers: { 'Content-Type': 'multipart/form-data' },
    })
    .then((r) => r.data)
}

/**
 * Aktualizuje istniejący przepis.
 * @param id ID przepisu
 * @param request Obiekt UpdateRecipeRequest (z opcjonalnym imageFile)
 */
export function updateRecipe(
  id: number,
  request: UpdateRecipeRequest & { imageFile?: File },
): Promise<RecipeResponse> {
  const form = new FormData()
  form.append('Name', request.name)
  if (request.defaultQty != null) form.append('DefaultQty', formatNumberForApi(request.defaultQty))
  if (request.defaultUnitId != null) form.append('DefaultUnitId', String(request.defaultUnitId))
  if (request.dishType) form.append('DishType', request.dishType)
  form.append('Description', request.description)

  // flaga usunięcia głównego obrazka
  if (request.removeImage) form.append('RemoveImage', 'true')
  if (request.imageFile) form.append('ImageFile', request.imageFile)

  // składniki (nadpisanie)
  request.ingredients.forEach((ing, i) => {
    form.append(`Ingredients[${i}].IngredientId`, String(ing.ingredientId))
    form.append(`Ingredients[${i}].Qty`, formatNumberForApi(ing.qty))
    form.append(`Ingredients[${i}].UnitId`, String(ing.unitId))
  })

  // kroki – UpdateInstructionStepRequest
  request.steps.forEach((st, i) => {
    form.append(`Steps[${i}].StepOrder`, String(st.stepOrder))
    form.append(`Steps[${i}].TemplateText`, st.templateText)
    form.append(`Steps[${i}].Id`, String(st.id))
    if (st.image) form.append(`Steps[${i}].Image`, st.image)
    if (st.removeImage) form.append(`Steps[${i}].RemoveImage`, 'true')
    if (st.imageFile) form.append(`Steps[${i}].ImageFile`, st.imageFile)
    ;(st.stepIngredients ?? []).forEach((psi, j) => {
      form.append(`Steps[${i}].StepIngredients[${j}].IngredientId`, String(psi.ingredientId))
      form.append(`Steps[${i}].StepIngredients[${j}].Fraction`, formatNumberForApi(psi.fraction))
    })
  })

  return api
    .put<RecipeResponse, AxiosResponse<RecipeResponse>, FormData>(`/recipes/${id}`, form, {
      headers: { 'Content-Type': 'multipart/form-data' },
    })
    .then((r) => r.data)
}

/**
 * Usuwa przepis.
 */
export function deleteRecipe(id: number): Promise<void> {
  return api.delete<void>(`/recipes/${id}`).then(() => {})
}

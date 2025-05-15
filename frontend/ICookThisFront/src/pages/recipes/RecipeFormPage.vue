<template>
  <section class="recipe-form-page">
    <h1>{{ isEdit ? 'Edit' : 'Create' }} Recipe</h1>

    <form @submit.prevent="onSubmit" enctype="multipart/form-data">
      <div class="field">
        <label>Name</label>
        <input v-model="form.name" required maxlength="200" />
      </div>

      <div class="field">
        <label>Default Quantity</label>
        <input type="number" v-model.number="form.defaultQty" min="0.001" step="0.001" required />
      </div>

      <div class="field">
        <label>Default Unit</label>
        <select v-model.number="form.defaultUnitId" required>
          <option value="" disabled>— wybierz jednostkę —</option>
          <option v-for="u in allUnits" :key="u.id" :value="u.id">
            {{ u.symbol }}
          </option>
        </select>
        <span v-if="form.defaultUnitId">(Typ: {{ getUnitType(form.defaultUnitId) }})</span>
      </div>

      <div class="field">
        <label>Dish Type</label>
        <select v-model="form.dishType" required>
          <option v-for="dt in dishTypes" :key="dt" :value="dt">{{ dt }}</option>
        </select>
      </div>

      <div class="field">
        <label>Description</label>
        <textarea v-model="form.description" required></textarea>
      </div>

      <div class="field">
        <label>Image</label>
        <input type="file" @change="onFileChange" />
        <label v-if="isEdit && form.imageUrl">
          <input type="checkbox" v-model="form.removeImage" />
          Remove current image
        </label>
      </div>

      <h2>Ingredients</h2>
      <div v-for="(ing, idx) in form.ingredients" :key="idx" class="subfield">
        <select v-model.number="ing.ingredientId" required>
          <option value="" disabled>— wybierz składnik —</option>
          <option v-for="item in allIngredients" :key="item.id" :value="item.id">
            {{ item.name }}
          </option>
        </select>

        <input type="number" v-model.number="ing.qty" placeholder="Qty" required step="0.001" />

        <select v-model.number="ing.unitId" required>
          <option value="" disabled>— wybierz jednostkę —</option>
          <option v-for="u in allUnits" :key="u.id" :value="u.id">
            {{ u.symbol }}
          </option>
        </select>
        <span v-if="ing.unitId">(Typ: {{ getUnitType(ing.unitId) }})</span>

        <button type="button" @click="removeIngredient(idx)">✕</button>
      </div>
      <button type="button" @click="addIngredient">Add ingredient</button>

      <h2>Steps</h2>
      <div v-for="(st, si) in form.steps" :key="si" class="subfield">
        <input type="number" v-model.number="st.stepOrder" placeholder="Order" required />
        <textarea v-model="st.templateText" placeholder="Template text" required></textarea>

        <div v-if="isEdit && 'imageUrl' in st && st.imageUrl" class="step-image-preview">
          <img :src="st.imageUrl" alt="step image" style="max-width: 120px" />
          <label>
            <input type="checkbox" v-model="st.removeImage" />
            Remove image
          </label>
        </div>

        <input type="file" @change="(e) => onStepFileChange(e, si)" />

        <div v-for="(psi, pi) in st.stepIngredients" :key="pi" class="step-ing">
          <select v-model.number="psi.ingredientId" required>
            <option value="" disabled>— wybierz składnik —</option>
            <option v-for="item in allIngredients" :key="item.id" :value="item.id">
              {{ item.name }}
            </option>
          </select>

          <input
            type="number"
            v-model.number="psi.fraction"
            placeholder="Fraction"
            required
            step="0.0001"
          />
          <button type="button" @click="removeStepIngredient(si, pi)">✕</button>
        </div>

        <button type="button" @click="addStepIngredient(si)">Add step ingredient</button>
        <button type="button" @click="removeStep(si)">Remove step</button>
      </div>
      <button type="button" @click="addStep">Add step</button>

      <div class="actions">
        <button type="submit" :disabled="submitting || (isEdit ? updateLoading : createLoading)">
          {{ isEdit ? 'Update' : 'Create' }}
        </button>
        <button
          type="button"
          @click="cancel"
          :disabled="submitting || (isEdit ? updateLoading : createLoading)"
        >
          Cancel
        </button>
      </div>

      <div v-if="isEdit ? updateError : createError" class="error">
        {{ (isEdit ? updateError : createError)!.message }}
      </div>
    </form>
  </section>
</template>

<script setup lang="ts">
import { reactive, ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import type { NewRecipeRequest, UpdateRecipeRequest } from '@/entities/recipe/models/recipeModel'
import type { RecipeIngredientRequest } from '@/entities/recipe/models/recipeIngredientModel'
import type {
  NewInstructionStepRequest,
  UpdateInstructionStepRequest,
} from '@/entities/recipe/models/instructionStepModel'
import { fetchRecipe } from '@/entities/recipe/services/recipeService'
import { useCreateRecipe, useUpdateRecipe } from '@/features/recipes/useMutateRecipe'

import { fetchIngredients } from '@/entities/ingredient/services/ingredientService'
import { fetchUnits } from '@/entities/unit/services/unitService'
import type { IngredientResponse } from '@/entities/ingredient/models/ingredientModel'
import type { UnitResponse } from '@/entities/unit/models/unitModel'

type RecipeStepForm =
  | NewInstructionStepRequest
  | (UpdateInstructionStepRequest & {
      imageUrl?: string
      removeImage: boolean
    })

type RecipeFormData = {
  name: string
  defaultQty: number
  defaultUnitId: number
  dishType: 'Appetizer' | 'MainCourse' | 'Dessert' | 'Snack' | 'Beverage'
  description: string
  imageFile?: File
  imageUrl?: string
  removeImage: boolean
  ingredients: RecipeIngredientRequest[]
  steps: RecipeStepForm[]
}

const router = useRouter()
const route = useRoute()
const id = Number(route.params.id)
const isEdit = Boolean(route.params.id)

const dishTypes = ['Appetizer', 'MainCourse', 'Dessert', 'Snack', 'Beverage'] as const

const allIngredients = ref<IngredientResponse[]>([])
const allUnits = ref<UnitResponse[]>([])

const form = reactive<RecipeFormData>({
  name: '',
  defaultQty: 1,
  defaultUnitId: 1,
  dishType: 'Dessert',
  description: '',
  ingredients: [],
  steps: [],
  removeImage: false,
})

const submitting = ref(false)

const {
  loading: createLoading,
  error: createError,
  result: createResult,
  exec: createExec,
} = useCreateRecipe()
const {
  loading: updateLoading,
  error: updateError,
  result: updateResult,
  exec: updateExec,
} = useUpdateRecipe()

function getUnitType(unitId: number): string | undefined {
  return allUnits.value.find((u) => u.id === unitId)?.type
}

function onFileChange(e: Event) {
  const file = (e.target as HTMLInputElement).files?.[0]
  if (file) form.imageFile = file
}

function onStepFileChange(e: Event, idx: number) {
  const file = (e.target as HTMLInputElement).files?.[0]
  if (file) form.steps[idx].imageFile = file
}

function addIngredient() {
  form.ingredients.push({ ingredientId: 0, qty: 1, unitId: 0 })
}
function removeIngredient(i: number) {
  form.ingredients.splice(i, 1)
}

function addStep() {
  form.steps.push({
    stepOrder: form.steps.length + 1,
    templateText: '',
    stepIngredients: [],
  } as NewInstructionStepRequest)
}
function removeStep(i: number) {
  form.steps.splice(i, 1)
}

function addStepIngredient(si: number) {
  form.steps[si].stepIngredients!.push({ ingredientId: 0, fraction: 1 })
}
function removeStepIngredient(si: number, pi: number) {
  form.steps[si].stepIngredients!.splice(pi, 1)
}

async function onSubmit() {
  submitting.value = true
  try {
    const stepsPayload = form.steps.map((st) => {
      const common = {
        stepOrder: st.stepOrder,
        templateText: st.templateText,
        stepIngredients: st.stepIngredients,
      }
      if ('id' in st) {
        return {
          id: st.id,
          ...common,
          image: st.imageUrl,
          removeImage: st.removeImage,
          imageFile: st.imageFile,
        } as UpdateInstructionStepRequest
      } else {
        return {
          ...common,
          imageFile: st.imageFile,
        } as NewInstructionStepRequest
      }
    })

    if (isEdit) {
      const payload: UpdateRecipeRequest & { imageFile?: File } = {
        name: form.name,
        defaultQty: form.defaultQty,
        defaultUnitId: form.defaultUnitId,
        dishType: form.dishType,
        description: form.description,
        imageFile: form.imageFile,
        removeImage: form.removeImage,
        ingredients: form.ingredients,
        steps: stepsPayload as UpdateInstructionStepRequest[],
      }
      await updateExec(id, payload)
      router.push(`/recipes/${updateResult.value!.id}`)
    } else {
      const payload: NewRecipeRequest & { imageFile?: File } = {
        name: form.name,
        defaultQty: form.defaultQty,
        defaultUnitId: form.defaultUnitId,
        dishType: form.dishType,
        description: form.description,
        imageFile: form.imageFile,
        ingredients: form.ingredients,
        steps: stepsPayload as NewInstructionStepRequest[],
      }
      await createExec(payload)
      router.push(`/recipes/${createResult.value!.id}`)
    }
  } finally {
    submitting.value = false
  }
}

onMounted(async () => {
  const paged = await fetchIngredients()
  allIngredients.value = paged.items
  allUnits.value = await fetchUnits()

  if (isEdit) {
    const recipe = await fetchRecipe(id)

    Object.assign(form, recipe, {
      imageFile: undefined,
      imageUrl: recipe.image,
      removeImage: false,
    })

    form.ingredients = recipe.ingredients.map((ri) => ({
      ingredientId: ri.ingredient.id,
      qty: ri.qty,
      unitId: ri.unit.id,
    }))

    form.steps = recipe.steps.map((st) => ({
      id: st.id,
      stepOrder: st.stepOrder,
      templateText: st.text,
      imageUrl: st.image,
      removeImage: false,
      stepIngredients: st.stepIngredients.map((psi) => ({
        ingredientId: psi.ingredient.id,
        fraction: psi.fraction,
      })),
    }))
  }
})

function cancel() {
  router.back()
}
</script>

<style scoped>
.recipe-form-page {
  max-width: 600px;
  margin: auto;
  padding: 1rem;
}
.field,
.subfield {
  margin-bottom: 0.75rem;
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
}
.field label {
  width: 120px;
  font-weight: bold;
}
.field input,
.field textarea,
.field select {
  flex: 1;
}
.actions {
  margin-top: 1rem;
  display: flex;
  gap: 0.5rem;
}
.error {
  color: red;
  margin-top: 1rem;
}
.step-ing {
  display: flex;
  gap: 0.5rem;
  align-items: center;
}
.step-image-preview {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}
</style>

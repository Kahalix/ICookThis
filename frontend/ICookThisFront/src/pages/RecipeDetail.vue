<template>
  <div v-if="recipe" class="recipe-detail">
    <h1>{{ recipe.name }}</h1>
    <img :src="`/${recipe.image}`" alt="" class="main-image" />

    <p><strong>Description:</strong> {{ recipe.description }}</p>
    <p>
      <strong>Default Qty:</strong>
      {{ (recipe.defaultQty * scale).toFixed(2) }} {{ recipe.defaultUnit.symbol }} (scale:
      <input type="number" v-model.number="scale" min="0.1" step="0.1" @change="reload" />)
    </p>

    <h2>Ingredients</h2>
    <ul>
      <li v-for="ri in recipe.ingredients" :key="ri.id">
        {{ (ri.qty * scale).toFixed(3) }} {{ ri.unit.symbol }}
        {{ ri.ingredient.name }}
      </li>
    </ul>

    <h2>Instructions</h2>
    <div v-for="step in recipe.steps" :key="step.id" class="step">
      <h3>Step {{ step.stepOrder }}</h3>
      <img :src="`/${step.image}`" v-if="step.image" class="step-image" />
      <p>{{ step.text }}</p>
    </div>
  </div>
  <div v-else>
    <p>Loading…</p>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import type { RecipeResponse } from '@/entities/recipe/models/recipeModel'
import { fetchRecipe } from '@/entities/recipe/services/recipeService'

const route = useRoute()
const id = Number(route.params.id)
const recipe = ref<RecipeResponse | null>(null)
const scale = ref(1)

async function reload() {
  recipe.value = await fetchRecipe(id, scale.value)
}

onMounted(reload)
</script>

<style scoped>
.recipe-detail {
  max-width: 600px;
  margin: auto;
}
.main-image,
.step-image {
  display: block;
  max-width: 100%;
  margin: 0.5em 0;
}
.step {
  border-top: 1px solid #ddd;
  padding-top: 1em;
}
</style>

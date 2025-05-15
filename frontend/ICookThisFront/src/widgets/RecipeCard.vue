<template>
  <div class="card">
    <div class="card__image-wrapper">
      <img
        v-if="recipe.image"
        :src="imageUrl"
        :alt="`Image of ${recipe.name}`"
        class="card__image"
      />
      <div v-else class="card__image--placeholder">No image</div>
    </div>
    <div class="card__body">
      <h2 class="card__title">{{ recipe.name }}</h2>
      <p class="card__meta">
        {{ recipe.defaultQty }}
        <span v-if="recipe.defaultUnit">{{ recipe.defaultUnit.symbol }}</span>
        <span v-else>—</span>
        – {{ recipe.dishType }}
      </p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { RecipeResponse } from '@/entities/recipe/models/recipeModel'

const props = defineProps<{ recipe: RecipeResponse }>()

const imageUrl = computed(() => {
  return props.recipe.image?.startsWith('/') ? props.recipe.image : `/${props.recipe.image}`
})
</script>

<style scoped>
.card {
  border: 1px solid #ddd;
  border-radius: 4px;
  overflow: hidden;
  max-width: 300px;
  background: #fff;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.card__image-wrapper {
  width: 100%;
  height: 200px;
  background: #f5f5f5;
  display: flex;
  align-items: center;
  justify-content: center;
}

.card__image {
  max-width: 100%;
  max-height: 100%;
  object-fit: cover;
}

.card__image--placeholder {
  color: #888;
  font-size: 0.9rem;
}

.card__body {
  padding: 0.5rem 1rem 1rem;
}

.card__title {
  margin: 0.5rem 0;
  font-size: 1.25rem;
}

.card__meta {
  margin: 0;
  color: #555;
  font-size: 0.9rem;
}
</style>

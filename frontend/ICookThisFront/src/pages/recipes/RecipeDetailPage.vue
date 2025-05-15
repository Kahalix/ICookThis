<template>
  <section class="recipe-detail-page">
    <button @click="goBack">← Back</button>

    <!-- Loader podczas fetching lub deleting -->
    <Loader v-if="loading || deleteLoading" />

    <!-- Błędy fetching lub deleting -->
    <div v-else-if="error" class="error">{{ error.message }}</div>
    <div v-else-if="deleteError" class="error">{{ deleteError.message }}</div>

    <div v-else-if="recipe">
      <h1>{{ recipe.name }}</h1>
      <img v-if="recipe.image" :src="`/${recipe.image}`" :alt="recipe.name" class="hero-img" />
      <p><strong>Serves:</strong> {{ recipe.defaultQty }} {{ recipe.defaultUnit.symbol }}</p>
      <p><strong>Type:</strong> {{ recipe.dishType }}</p>
      <p><strong>Description:</strong> {{ recipe.description }}</p>

      <h2>Ingredients</h2>
      <ul>
        <li v-for="ri in recipe.ingredients" :key="ri.id">
          {{ ri.qty }} {{ ri.unit.symbol }} {{ ri.ingredient.name }}
        </li>
      </ul>

      <h2>Steps</h2>
      <ol>
        <li v-for="step in recipe.steps" :key="step.id">
          <img v-if="step.image" :src="`/${step.image}`" class="step-img" />
          {{ step.text }}
        </li>
      </ol>

      <div class="actions">
        <router-link :to="`/recipes/${recipe.id}/edit`" class="btn">Edit</router-link>
        <button @click="onDelete" class="btn btn-delete">Delete</button>
      </div>
    </div>

    <div v-else>Not found.</div>
  </section>
</template>

<script setup lang="ts">
import { onMounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import Loader from '@shared/ui/Loader.vue'
import { useFetchRecipe } from '@features/recipes/useFetchRecipe'
import { useDeleteRecipe } from '@features/recipes/useMutateRecipe'

const route = useRoute()
const router = useRouter()
const id = Number(route.params.id)

const { loading, data: recipe, error, load } = useFetchRecipe()

const {
  loading: deleteLoading,
  error: deleteError,
  success: deleteSuccess,
  exec: deleteExec,
} = useDeleteRecipe()

function goBack() {
  //router.back()
  router.push({ name: 'RecipeList' })
}

function onDelete() {
  if (!recipe.value) return

  const name = recipe.value.name
  if (window.confirm(`Czy na pewno chcesz usunąć przepis "${name}"?`)) {
    deleteExec(recipe.value.id)
  }
}

watch(deleteSuccess, (val: boolean) => {
  if (val) {
    router.push({ name: 'RecipeList' })
  }
})

onMounted(() => {
  load(id)
})
</script>

<style scoped>
.recipe-detail-page {
  max-width: 700px;
  margin: auto;
  padding: 1rem;
}
.hero-img {
  width: 100%;
  max-height: 300px;
  object-fit: cover;
  margin-bottom: 1rem;
}
.step-img {
  width: 100%;
  max-width: 400px;
  display: block;
  margin: 0.5rem 0;
}
.btn {
  display: inline-block;
  margin-top: 1rem;
  padding: 0.4rem 0.8rem;
  background: #42b983;
  color: #fff;
  text-decoration: none;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}
.btn-delete {
  background: #e74c3c;
  margin-left: 0.5rem;
}
.actions {
  margin-top: 1rem;
}
.error {
  color: red;
}
</style>

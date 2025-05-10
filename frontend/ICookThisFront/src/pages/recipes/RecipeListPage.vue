<template>
  <section class="recipe-list-page">
    <h1>Recipes</h1>
    <div class="controls">
      <input v-model="search" placeholder="Search…" @input="reload" />
      <button @click="prev" :disabled="page <= 1">←</button>
      <span>Page {{ page }} / {{ data.totalPages }}</span>
      <button @click="next" :disabled="page >= data.totalPages">→</button>
      <router-link to="/recipes/new" class="btn">New Recipe</router-link>
    </div>

    <Loader v-if="loading" />
    <div v-else class="cards">
      <RecipeCard v-for="r in data.items" :key="r.id" :recipe="r" @click="goDetail(r.id)" />
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import Loader from '@shared/ui/Loader.vue'
import RecipeCard from '@widgets/RecipeCard.vue'
import { useFetchRecipes } from '@features/recipes/useFetchRecipes'

const router = useRouter()
const { loading, data, load } = useFetchRecipes()
const page = ref(1)
const search = ref('')

function reload() {
  load(page.value, 10, search.value)
}
function next() {
  page.value++
  reload()
}
function prev() {
  page.value--
  reload()
}
function goDetail(id: number) {
  router.push(`/recipes/${id}`)
}

onMounted(reload)
</script>

<style scoped>
.recipe-list-page .controls {
  display: flex;
  gap: 0.5rem;
  align-items: center;
  margin-bottom: 1rem;
}
.cards {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  gap: 1rem;
}
.btn {
  margin-left: auto;
  padding: 0.3rem 0.6rem;
  background: #42b983;
  color: white;
  text-decoration: none;
  border-radius: 4px;
}
</style>

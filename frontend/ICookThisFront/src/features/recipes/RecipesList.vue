<!-- <template>
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

    <div v-else-if="error" class="error">
      <p>Error: {{ error.message }}</p>
    </div>

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
const { loading, error, data, load } = useFetchRecipes()
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
</script> -->

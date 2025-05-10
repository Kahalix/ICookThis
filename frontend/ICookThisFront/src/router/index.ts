import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'
import RecipeListPage from '@pages/recipes/RecipeListPage.vue'
import RecipeDetailPage from '@pages/recipes/RecipeDetailPage.vue'
import RecipeFormPage from '@pages/recipes/RecipeFormPage.vue'

const routes: Array<RouteRecordRaw> = [
  { path: '/', redirect: '/recipes' },

  // lista przepisów
  {
    path: '/recipes',
    name: 'RecipeList',
    component: RecipeListPage,
  },

  // szczegóły przepisu
  {
    path: '/recipes/:id',
    name: 'RecipeDetail',
    component: RecipeDetailPage,
    props: (route) => ({ id: Number(route.params.id) }),
  },

  // formularz tworzenia nowego przepisu
  {
    path: '/recipes/new',
    name: 'RecipeCreate',
    component: RecipeFormPage,
  },

  // formularz edycji przepisu
  {
    path: '/recipes/:id/edit',
    name: 'RecipeEdit',
    component: RecipeFormPage,
    props: (route) => ({ id: Number(route.params.id) }),
  },
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
})

export default router

<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
</script>

<template>
  <div id="app">
    <header class="app-header">
      <h1>🍳 ICookThis</h1>
      <nav>
        <RouterLink to="/recipes" class="nav-link">Recipes</RouterLink>
        <RouterLink to="/recipes/new" class="nav-link">New Recipe</RouterLink>
      </nav>
    </header>

    <main class="app-main">
      <RouterView />
    </main>

    <footer class="app-footer">© 2025 ICookThis</footer>
  </div>
</template>

<style scoped>
#app {
  display: flex;
  flex-direction: column;
  height: 100vh;
}
.app-header {
  background: #42b983;
  padding: 1rem;
  color: white;
}
.app-header h1 {
  margin: 0;
  font-size: 1.5rem;
  display: inline-block;
}
nav {
  display: inline-block;
  margin-left: 2rem;
}
.nav-link {
  margin-right: 1rem;
  color: white;
  text-decoration: none;
}
.nav-link.router-link-exact-active {
  font-weight: bold;
  text-decoration: underline;
}

.app-main {
  flex: 1;
  overflow: auto;
  padding: 1rem;
}

.app-footer {
  text-align: center;
  padding: 0.5rem;
  background: #f5f5f5;
  font-size: 0.9rem;
}
</style>

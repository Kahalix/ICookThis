<!-- eslint-disable vue/multi-word-component-names -->
<template>
  <div class="loader">
    <div class="spinner"></div>
    <p>Loading...</p>
  </div>
</template>

<style scoped>
.loader {
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-top: 2rem;
}
.spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #ccc;
  border-top-color: #333;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}
@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}
</style>

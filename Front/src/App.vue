<template>
  <nav class="navbar navbar-expand-lg navbar-light bg-light">
    <div class="container-fluid">
      <RouterLink class="navbar-brand" to="/">Congratulate</RouterLink>
      <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
        <span class="navbar-toggler-icon"></span>
      </button>
      <div class="collapse navbar-collapse" id="navbarNav">
        <ul class="navbar-nav">
          <li class="nav-item">
            <RouterLink class="nav-link" :class="{ active: $route.path === '/' }" to="/">Главная</RouterLink>
          </li>
          <li class="nav-item">
            <RouterLink class="nav-link" :class="{ active: $route.path === '/create' }" to="/create">Добавить ДР</RouterLink>
          </li>
        </ul>
      </div>
    </div>
  </nav>
  <div class="container mt-4">
    <RouterView />
  </div>
</template>

<script setup>

</script>

<style>
.container {
  max-width: 900px;
}
</style>
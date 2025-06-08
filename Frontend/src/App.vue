<template>
  <!-- бульки під усім -->
  <Background />

  <!-- контент трохи «піднятий» над фоном -->
  <main style="position:relative; z-index:2; padding:2rem; color:#fff;">
    <InstallerList/>
  </main>
</template>

<script setup lang="ts">
  import InstallerList from './components/InstallerList.vue';
  import Background from './components/Background.vue'
  import './styles/styles.css'
  import './styles/background.css'
</script>

<style scoped>
</style>
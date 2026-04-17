<script setup>
  import { computed } from 'vue'
  import ImageUpload from './components/ImageUpload.vue'
  import Navbar from './components/Navbar.vue'
  import { store } from '../store.js'
</script>

<template>
  <div class="position" :class="store.isDarkMode ? 'darkMode' : 'lightMode' ">
    <Navbar />
    <ImageUpload />
  </div>
</template>

<style scoped>
  header {
    line-height: 1.5;
  }

  .position {
    min-height: 100vh;
    min-width: 100vw;
    margin: 0;
    padding: 0;
    box-sizing: border-box;
    transition: color 0.5s ease;
    transition: background-color 0.5s ease;
  }

  .darkMode {
    background-color: #181818;
    color: white;
  }

  .lightMode {
    background-color: #fafafa;
    color: black;
  }

  @media (min-width: 1024px) {
    header {
      display: flex;
      place-items: center;
      padding-right: calc(var(--section-gap) / 2);
    }

    .logo {
      margin: 0 2rem 0 0;
    }

    header .wrapper {
      display: flex;
      place-items: flex-start;
      flex-wrap: wrap;
    }
  }
</style>

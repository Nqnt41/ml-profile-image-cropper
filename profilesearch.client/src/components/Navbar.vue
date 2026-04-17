<script setup lang="js">
  import { store } from '../../store.js'
</script>

<template>
  <div class="navbar" :class="store.isDarkMode ? 'darkNav' : 'lightNav'">
    <h1>Home</h1>
    <button @click="setDarkMode">Dark Mode</button>
  </div>
</template>

<script lang="js">
  import { defineComponent, getCurrentInstance } from 'vue';

  export default defineComponent({
    data() {
      return {
        test: 0,
      };
    },
    watch: {

    },
    methods: {
      setDarkMode() {
        store.isDarkMode = !store.isDarkMode;
      }
    }
  });
</script>

<style scoped>
  .navbar {
    display: flex;
    width: 100vw;
    max-height: 10vh;
    padding: 1rem 2rem;
    align-content: center;
    transition: color 0.5s ease;
    transition: background-color 0.5s ease;
  }

  .darkNav {
    background: #404040;
  }

  .lightNav {
    background: #d2d3bd;
  }

  .rightAlign {
    justify-content: flex-end;
  }
</style>

<script setup lang="js">
  import { getCurrentInstance } from 'vue'
  import CropImage from './CropImage.vue'
  import { store } from '../../store.js'

  console.log("store", store);
</script>

<template>
  <div class="containerPosition" :class="store.isDarkMode ? 'containerDarkMode' : 'containerLightMode'">
    <h1>Image Cropper</h1>
    <p>Part One of this project will, when done, let the user crop an image automatically based on the bounds found by facial recognition.</p>
    <ol>
      <li>Start with image downloading and manual cropping with a download button.</li>
      <li>Then, set up Entity infrastructure to store images and such</li>
      <li>Start setting up neural network that does two reads - one to determine if character is fictional or not, two to detemine bounds using correct dataset</li>
      <li>Can do a user system after that - add and remove user, save the pfps they create</li>
    </ol>

    <CropImage />
  </div>
</template>

<script lang="js">
    import { defineComponent } from 'vue';
    import Cropper from 'cropperjs';

    export default defineComponent({
        data() {
            return {
                loading: false,
                post: null
            };
        },
        async created() {
            // fetch the data when the view is created and the data is
            // already being observed
            //await this.fetchData();
        },
        watch: {
            // call again the method if the route changes
            '$route': 'fetchData'
        },
        methods: {
            /*async fetchData() {
                this.post = null;
                this.loading = true;

                var response = await fetch('weatherforecast');
                if (response.ok) {
                    this.post = await response.json();
                    this.loading = false;
                }
            }*/
        },
    });
</script>

<style scoped>
  .containerPosition {
    max-width: 1280px;
    margin: 2rem auto;
    padding: 2rem;
    border-radius: 20px;
  }

  .containerDarkMode {
    background-color: #212121;
  }

  .containerLightMode {
    background-color: #e4e5f1;
  }
</style>

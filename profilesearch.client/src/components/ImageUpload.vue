<script setup lang="js">
  import CropImage from './CropImage.vue'
</script>

<template>
  <div>
    <h1>Image Cropper</h1>
    <p>Part One of this project will, when done, let the user crop an image automatically based on the bounds found by facial recognition.</p>
    <ol>
      <li>Start with image downloading and manual cropping with a download button.</li>
      <li>Then, set up Entity infrastructure to store images and such</li>
      <li>Start setting up neural network that does two reads - one to determine if character is fictional or not, two to detemine bounds using correct dataset</li>
      <li>Can do a user system after that - add and remove user, save the pfps they create</li>
    </ol>

    <CropImage />

    <!--<tr v-for="forecast in post" :key="forecast.date">
      <td>{{ forecast.date }}</td>
      <td>{{ forecast.temperatureC }}</td>
      <td>{{ forecast.temperatureF }}</td>
      <td>{{ forecast.summary }}</td>
    </tr>-->
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

</style>

<script setup lang="js">import { getCurrentInstance, provide, ref, inject } from 'vue'
  import CropImage from './CropImage.vue'
  import { store } from '../../store.js'
  import axios from 'axios';

  console.log("store", store);
</script>

<template>
  <div>
    <label for="searchbar">Search for Existing Image</label>
  </div>
  <form @submit.prevent="handleSubmit" enctype="multipart/form-data">
    <input id="searchbar" name="searchbar" type="text" v-model="input" placeholder="Search by character..." @keyup.enter="handleSubmit" />
    <button type="submit">Submit</button>
  </form>
  <div>
    <img id="pulledImage" />
  </div>
</template>

<script lang="js">
  import { defineComponent } from 'vue';

  const input = ref('');

  export default defineComponent({
    data() {
      return {
        loading: false,
        post: null,
        existingCharactersMap: new Map()
      };
    },
    async beforeMount() {
      console.log('beforeMount called - starting to populate map');
      await this.handleMapPopulate();
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
      async handleMapPopulate() {
        var res = new Map();
        var pairs = [];
        let getPairsRes = null;

        let baseDelay = 500;
        let delay = 500;

        for (let i = 0; i < 10; i++) {
          try {
            getPairsRes = await axios.get(`/api/images/getPairs`);

            if (getPairsRes.status == 200) {
              console.log("handleMapPopulate response:", getPairsRes);
              pairs = getPairsRes.data;
              break;
            }
          }
          catch (e) {
            const status = e.response?.status;

            if (status == 502 || status == 503) {
              console.log(`Backend not ready (${status}), retrying... (attempt ${i + 1}/10)`);
              delay = baseDelay * Math.pow(2, i);
              await new Promise(resolve => setTimeout(resolve, delay));
            }
            else {
              console.error("ERROR IN handleMapPopulate - invalid status message during attempt loop:", status, e);
              break;
            }
          }
        }

        if (getPairsRes == null || getPairsRes?.status != 200) {
          console.error("ERROR IN handleMapPopulate - backend query not completed in allotted attempts", getPairsRes);
        }

        for (let pair of pairs) {
          res.set(pair.name, pair.id);
        }

        this.existingCharactersMap = res;
      },
      async handleSubmit() {
        if (this.existingCharactersMap !== undefined) {
          const mapOutput = this.existingCharactersMap.get(input.value.toLowerCase());

          if (input.value != '' && mapOutput != undefined) {
            let img = document.getElementById("pulledImage");

            await axios.get(`/api/images/${mapOutput}`)
              .then(response => {
                console.log("handleSubmit response:", response);
                img.src = response.data.url;
              }).catch(e => {
                console.error("ERROR IN handleSubmit -", e);
              })
          }
          else {
            console.warn('No match found for:', input.value);
          }
        }
        else {
          console.warn('existingCharactersMap is undefined');
        }
      }
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

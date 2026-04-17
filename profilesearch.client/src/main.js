import './assets/main.css'

import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap/dist/js/bootstrap.bundle.min.js'

import 'cropperjs'

import 'croppie/croppie.css';

import { createApp, reactive } from 'vue'
import App from './App.vue'

const app = createApp(App);

const globalState = reactive({
  isDarkMode: true
});

app.config.globalProperties.$globalState = globalState;

app.mount('#app')

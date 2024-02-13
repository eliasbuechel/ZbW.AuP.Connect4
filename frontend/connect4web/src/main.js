import { createApp } from 'vue'
import App from './App.vue'
import signalRService from './signalRService';

const app = createApp(App);
app.use(signalRService);
signalRService.start();

app.mount('#app');
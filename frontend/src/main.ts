// Externe Bibliotheken/Frameworks
import { Router, createRouter, createWebHistory } from "vue-router";
import axios, { Axios, AxiosInstance } from "axios";

// Interne Module
import { createApp, App, provide } from "vue";
import AppVue from "./App.vue";

// Lokale Dateien/Komponenten
import HomeView from "./views/HomeView.vue";
import Login from "components/LoginForm.vue";
import Register from "components/RegisterForm.vue";
import GameView from "views/GameView.vue";

const router: Router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: "/", name: "Home", component: HomeView },
    { path: "/login", name: "Login", component: Login },
    { path: "/register", name: "Register", component: Register },
    { path: "/game", name: "Game", component: GameView },
  ],
});

const app: App = createApp(AppVue);

declare module "@vue/runtime-core" {
  interface ComponentCustomProperties {
    $router: Router;
    $axios: AxiosInstance;
  }
}

provide(Axios, axios);

app.config.globalProperties.$router = router;
app.config.globalProperties.$axios = axios as AxiosInstance;

app.use(router);
app.mount("#app");

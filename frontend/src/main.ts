// Externe Bibliotheken/Frameworks
import { Router, createRouter, createWebHistory } from "vue-router";
import axios, { AxiosStatic } from "axios";

// Interne Module
import { App, createApp } from "vue";
import AppVue from "./App.vue";

// Lokale Dateien/Komponenten
import HomeView from "@/views/HomeView.vue";
import Login from "@/components/LoginForm.vue";
import Register from "@/components/RegisterForm.vue";
import GameView from "./views/GameView.vue";
import EmailVerificationForm from "./components/EmailVerificationForm.vue";

const router: Router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: "/", name: "Home", component: HomeView },
    { path: "/login", name: "Login", component: Login },
    { path: "/register", name: "Register", component: Register },
    { path: "/game", name: "Game", component: GameView },
    { path: "/email-verification", name: "EmailVerificationForm", component: EmailVerificationForm}
  ],
});

const app: App = createApp(AppVue);

declare module "@vue/runtime-core" {
  interface ComponentCustomProperties {
    $axios: AxiosStatic;
  }
}

app.config.globalProperties.$axios = axios;

app.use(router);
app.mount("#app");

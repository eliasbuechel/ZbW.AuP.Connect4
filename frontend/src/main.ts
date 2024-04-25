// Externe Bibliotheken/Frameworks
import { Router, createRouter, createWebHistory } from "vue-router";
import axios, { AxiosStatic } from "axios";

// Interne Module
import { App, createApp } from "vue";
import AppVue from "./App.vue";

// Lokale Dateien/Komponenten
import HomeView from "@/views/HomeView.vue";
import LoginView from "./views/LoginView.vue";
import RegisterView from "./views/RegisterView.vue";

const router: Router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: "/", name: "Home", component: HomeView },
    { path: "/login", name: "Login", component: LoginView },
    { path: "/register", name: "Register", component: RegisterView },
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

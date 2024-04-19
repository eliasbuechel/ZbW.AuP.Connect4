// Externe Bibliotheken/Frameworks
import { createRouter, createWebHistory } from "vue-router";
import axios from "axios";

// Interne Module
import { createApp } from "vue";
import App from "./App.vue";

// Lokale Dateien/Komponenten
import HomeView from "@/views/HomeView";
import Login from "@/components/LoginForm.vue";
import Register from "@/components/RegisterForm.vue";

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: "/", name: "Home", component: HomeView },
    { path: "/login", name: "Login", component: Login },
    { path: "/register", name: "Register", component: Register },
  ],
});

const app = createApp(App);

app.config.globalProperties.$axios = axios;

app.use(router);
app.mount("#app");

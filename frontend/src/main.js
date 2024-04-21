// Externe Bibliotheken/Frameworks
import { createRouter, createWebHistory } from "vue-router";
import axios, { Axios } from "axios";
// Interne Module
import { createApp, provide } from "vue";
import AppVue from "./App.vue";
// Lokale Dateien/Komponenten
import HomeView from "./views/HomeView.vue";
import Login from "components/LoginForm.vue";
import Register from "components/RegisterForm.vue";
import GameView from "views/GameView.vue";
var router = createRouter({
    history: createWebHistory(),
    routes: [
        { path: "/", name: "Home", component: HomeView },
        { path: "/login", name: "Login", component: Login },
        { path: "/register", name: "Register", component: Register },
        { path: "/game", name: "Game", component: GameView },
    ],
});
var app = createApp(AppVue);
provide(Axios, axios);
app.config.globalProperties.$router = router;
app.config.globalProperties.$axios = axios;
app.use(router);
app.mount("#app");

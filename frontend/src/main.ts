// extern libraris/frameworks
import { Router, createRouter, createWebHistory } from "vue-router";
import axios, { AxiosStatic } from "axios";
import VueCookies from "vue-cookies";

// Intern modules
import { App, createApp } from "vue";
import AppVue from "./App.vue";
import routes from "@/routes";
import store from "@/services/store";

const router = createRouter({
  history: createWebHistory(),
  routes,
});
export default router;

router.beforeEach(async (to, from, next) => {
  if (to.meta.requiresAuth) {
    await store.dispatch("checkAuth");
    if (store.state.isAuthenticated) {
      next();
    } else {
      next("/login");
    }
  } else {
    next();
  }
});


// Register (main)App
const app: App = createApp(AppVue);

declare module "@vue/runtime-core" {
  interface ComponentCustomProperties {
    $axios: AxiosStatic;
  }
}
app.config.globalProperties.$axios = axios;
// axios.defaults.baseURL = "https://api.r4d4.work";
axios.defaults.baseURL = "http://localhost:8082";

app.use(router);
app.use(VueCookies);
app.mount("#app");

// extern libraris/frameworks
import { Router, createRouter, createWebHistory } from "vue-router";
import axios, { AxiosStatic } from "axios";

// Intern modules
import { App, createApp, nextTick } from "vue";
import AppVue from "./App.vue";
import routes from "@/routes";
import authService from "@/services/authService";

const router = createRouter({
  history: createWebHistory(),
  routes
});
export default router;

router.beforeEach((to, from, next) => {
  if (to.meta.requiresAuth && !authService.isAuthenticated()) {
    next('/login'); 
  } else if (to.name === "Login" && authService.isAuthenticated()) {
    next('/home'); 
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
axios.defaults.baseURL = 'http://localhost:5000';


app.use(router);
app.mount("#app");

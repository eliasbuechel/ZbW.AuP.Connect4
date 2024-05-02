// extern libraris/frameworks
import { Router, createRouter, createWebHistory } from "vue-router";
import axios, { AxiosStatic } from "axios";

// Intern modules
import { App, createApp } from "vue";
import AppVue from "./App.vue";
import routes from "@/routes";

const router = createRouter({
  history: createWebHistory(),
  routes
});
export default router;

// Navigation Guard
router.beforeEach((to, from, next) => {
  const requiresAuth = to.matched.some(record => record.meta.requiresAuth !== false);
  const isAuthenticated = localStorage.getItem('authToken'); 

  if (requiresAuth && !isAuthenticated) {
    next('/login'); 
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

app.use(router);
app.mount("#app");


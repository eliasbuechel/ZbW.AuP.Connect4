// extern libraris/frameworks
import { Router, createRouter, createWebHistory } from "vue-router";
import axios, { AxiosStatic } from "axios";

// Intern modules
import { App, createApp, nextTick } from "vue";
import AppVue from "./App.vue";
import routes from "@/routes";

const router = createRouter({
  history: createWebHistory(),
  routes
});
export default router;

router.beforeEach(async (to, from, next) =>{
  if (to.meta.requiresAuth) {
    try {
      const response = await axios.post("account/checkAuthentication");

      if (response.data.authenticated) {
        next();
      } else {
        next("/login");
      }
    } catch (error) {
      console.error("Error checking authentication status:", error);
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

app.use(router);
app.mount("#app");

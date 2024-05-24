import HomeView from "@/views/HomeView.vue";
import Login from "@/views/LoginView.vue";
import Register from "@/views/RegisterView.vue";


const routes = [
    { path: "/home", name: "Home", component: HomeView, meta: { requiresAuth: true }},
    { path: "/login", name: "Login", component: Login, meta: { requiresAuth: false } },
    { path: "/register", name: "Register", component: Register, meta: { requiresAuth: false }},
  ];
  
  export default routes;
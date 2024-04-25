import HomeView from "@/views/HomeView.vue";
import Login from "@/components/LoginForm.vue";
import Register from "@/components/RegisterForm.vue";
import GameView from "./views/GameView.vue";
import EmailVerificationForm from "./components/EmailVerificationForm.vue";


const routes = [
    { path: "/home", name: "Home", component: HomeView, meta: { requiresAuth: true }},
    { path: "/login", name: "Login", component: Login },
    { path: "/register", name: "Register", component: Register },
    { path: "/game", name: "Game", component: GameView },
    { path: "/email-verification", name: "EmailVerificationForm", component: EmailVerificationForm }
  ];
  
  export default routes;
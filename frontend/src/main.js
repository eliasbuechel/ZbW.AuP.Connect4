import { createApp } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'
import App from './App.vue'
import HomeView from '@/views/HomeView'
import Login from '@/components/LoginForm.vue'
import Register from '@/components/RegisterForm.vue'
import EmailVerification from '@/components/EmailVerifactionForm.vue'


import '@/assets/global.css'

const router = createRouter({
    history: createWebHistory(),
    routes: [
        {path: '/', name: 'Home', component: HomeView},
        {path: '/login', name: 'Login', component: Login},
        {path: '/register', name: 'Register', component: Register},
        {path: '/email-verification', name: 'EmailVerification', component: EmailVerification},
    ]
})

createApp(App)
.use(router)
.mount('#app')

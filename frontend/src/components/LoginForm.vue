<template>
    <img id="logo-login" src="" alt="r4d4-logo">
    <div class="login-form">
        <form>
            <div class="form-group">
                <label for="username">Benutzername / Email</label>
                <input type="text" id="username" v-model="credentials.username" required>
            </div>
            <div class="form-group">
                <label for="password">Passwort</label>
                <input type="password" id="password" v-model="credentials.password" required>
            </div>
            <button class="auth-button" type="button" @click="login">Anmelden</button>
            <router-link to="/register" class="auth-button">Registrieren</router-link>
        </form>
    </div>
</template>

<script>
export default {
    data() {
        return {
            credentials: {
                username: '',
                password: '',
            },
        };
    },
    methods: {
        login() {
            this.$axios.post('localhost/api/login', this.credentials)
                .then(response => {
                    localStorage.setItem('authToken', response.data.token);
                    this.$router.push({ name: 'Home' })
                })
                .catch(error => {
                    console.error("Fehler beim Einloggen:", error);
                });
        },
    },
};
</script>

<style scoped>
@import '/src/assets/authentication.css'
</style>
<template>
    <img id="logo" src="" alt="r4d4-logo">
    <div class="login-form">
        <form @submit.prevent="login">
            <div class="form-group">
                <label for="username">Benutzername / Email</label>
                <input type="text" id="username" v-model="credentials.username" required>
            </div>
            <div class="form-group">
                <label for="password">Passwort</label>
                <input type="password" id="password" v-model="credentials.password" required>
            </div>
            <button type="submit">Anmelden</button>
            <button type="submit">Registrieren</button>
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
            console.log("Anmeldeversuch mit:", this.credentials);
        },
    },
};
</script>

<style scoped>
@import '/src/assets/authentication.css'
</style>
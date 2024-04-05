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
.login-form {
    max-width: 400px;
    margin: 0 auto;
    padding: 20px;
}

.form-group {
    margin-bottom: 15px;
}

label {
    display: block;
}

input[type="text"],
input[type="password"] {
    width: 100%;
    padding: 8px;
    margin-top: 5px;
}

button[type="submit"] {
    width: 100%;
    margin: 5px;
    padding: 10px;
    background-color: #3498db;
    color: white;
    border: none;
    cursor: pointer;
}

button[type="submit"]:hover {
    background-color: #2980b9;
}
</style>
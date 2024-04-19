<template>
  <div class="login-container">
    <img id="logo-login" src="" alt="r4d4-logo" />
    <form @submit.prevent="login">
      <div class="input-field">
        <label for="username">Benutzername / Email</label>
        <input
          type="text"
          id="username"
          v-model="credentials.username"
          required
        />
      </div>
      <div class="input-field">
        <label for="password">Passwort</label>
        <input
          type="password"
          id="password"
          v-model="credentials.password"
          required
        />
      </div>
      <button class="button-submit" type="submit" @click="login">Login</button>
      <button class="button-link" type="button" @click="redirectToRegister">
        Switch to registration
      </button>
    </form>
  </div>
</template>

<script>
export default {
  data() {
    return {
      credentials: {
        username: "",
        password: "",
      },
    };
  },
  methods: {
    async login() {
      try {
        const response = await this.$axios.post(
          "http://localhost:5000/Account/Login",
          this.credentials
        );

        if (response.status >= 200 && response.status < 300) {
          this.$router.push({ name: "Home" });
        } else {
          console.error("Login failed:", response.statusText);
        }
      } catch (error) {
        console.error("An error occured during login: ", error.message);
      }
    },
    redirectToRegister() {
      this.$router.push({ name: "Register" });
    },
  },
};
</script>

<style scoped>
@import "/src/assets/authentication.css";
</style>

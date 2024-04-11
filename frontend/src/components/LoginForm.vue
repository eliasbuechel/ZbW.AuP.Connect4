<template>
  <div class="login-container">
    <img id="logo-login" src="" alt="r4d4-logo" />
    <form @submit.prevent="login"> <!-- Blocks default behavior of form, this case: reloading the page. -->
      <div class="form-group">
        <label for="username">Benutzername / Email</label>
        <input
          type="text"
          id="username"
          v-model="credentials.username"
          required
        />
      </div>
      <div class="form-group">
        <label for="password">Passwort</label>
        <input
          type="password"
          id="password"
          v-model="credentials.password"
          required
        />
      </div>
      <button class="auth-button" type="submit" @click="login">Login</button>
      <button class="auth-button" type="button" @click="register">
        Register
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
        const response = await this.$axios.post("http://localhost:5000/api/login", this.credentials);
      
        if (response.data.token) {
          localStorage.setItem("authToken", response.data.token);
          
          this.$router.push({ name: "Home" });
        } else {
          console.error("Ungültige Anmeldeinformationen");
        }
      } catch (error) {
        console.error("Fehler beim Einloggen:", error);
      }
    },
  },
};
</script>

<style scoped>
@import "/src/assets/authentication.css";
</style>

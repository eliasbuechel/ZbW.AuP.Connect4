<template>
  <div class="login-container">
    <img id="logo" src="" alt="r4d4-logo" />
    <div class="login-container">
      <form @submit.prevent="login">
        <div class="input-field">
          <label for="username">Username</label>
          <input
            type="text"
            id="register-username"
            v-model="credentials.username"
            required
          />
        </div>
        <div class="input-field">
          <label for="username">Email</label>
          <input
            type="text"
            id="register-email"
            v-model="credentials.email"
            required
          />
        </div>
        <div class="input-field">
          <label for="password">Password</label>
          <input
            type="password"
            id="register-password"
            v-model="credentials.password"
            required
          />
        </div>
        <button class="button-submit" type="submit" @click="register">
          Register
        </button>
        <button class="button-link" type="button" @click="redirectToLogin">
          Switch to login
        </button>
      </form>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      credentials: {
        username: "",
        email: "",
        password: "",
      },
    };
  },
  methods: {
    async register() {
      try {
        const response = await this.$axios.post(
          "http://localhost:5000/Account/Register",
          this.credentials
        );

        if (response.status >= 200 && response.status < 300) {
          this.$router.push({ name: "Home" });
        } else {
          console.error("Registration failed:", response.statusText);
        }
      } catch (error) {
        console.error("An error occured during registration: ", error.message);
      }
    },
    redirectToLogin() {
      this.$router.push({ name: "Login" });
    },
  },
};
</script>

<style scoped>
@import "/src/assets/authentication.css";
</style>

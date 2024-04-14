<template>
  <div class="login-container">
    <img id="logo" src="" alt="r4d4-logo" />
    <div class="login-container">
      <form @submit.prevent="login">
        <div class="input-field">
          <label for="username">Benutzername</label>
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
            v-model="credentials.username"
            required
          />
        </div>
        <div class="input-field">
          <label for="password">Passwort</label>
          <input
            type="password"
            id="register-password"
            v-model="credentials.password"
            required
          />
        </div>
        <button type="submit">Registrieren</button>
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
        password: "",
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
@import "/src/assets/authentication.css";
</style>

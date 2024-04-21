<template>
  <div class="login-container">
    <img id="logo-login" src="" alt="r4d4-logo" />
    <form @submit.prevent="login">
      <div class="input-field">
        <label for="email">Email</label>
        <input
          type="email"
          id="email"
          v-model="credentials.email"
          @focusout="validateEmail"
          required
        />
        <span v-if="errors.email" class="error">{{ errors.email }}</span>
      </div>
      <div class="input-field">
        <label for="password">Password</label>
        <input
          type="password"
          id="password"
          v-model="credentials.password"
          @focusout="validatePassword"
          required
        />
        <span v-if="errors.password" class="error">{{ errors.password }}</span>
      </div>
      <span v-if="errors.login" class="error">{{ errors.login }}</span>
      <button class="button-submit" :disabled="!allowLogin" type="submit">
        Login
      </button>
      <button class="button-link" type="button" @click="redirectToRegister">
        Registration
      </button>
    </form>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";

interface Credentials {
  email: String;
  password: String;
}

interface Errors {
  email: String;
  password: String;
  login: String;
}

export default defineComponent({
  name: "LoginForm",
  components: {},
  data(): { credentials: Credentials; errors: Errors } {
    return {
      credentials: {
        email: "",
        password: "",
      },
      errors: {
        email: "",
        password: "",
        login: "",
      },
    };
  },
  methods: {
    async login(): Promise<void> {
      try {
        await this.$axios.post(
          "http://localhost:5000/login?useCookies=true",
          this.credentials,
          {
            withCredentials: true,
          }
        );
        this.errors.login = "";
        this.$router.push({ name: "Home" });
      } catch (error: any) {
        this.errors.login = error.message;
      }
    },
    async validateEmail(): Promise<void> {
      const emailInput: HTMLInputElement = document.getElementById(
        "email"
      ) as HTMLInputElement;
      if (!emailInput.checkValidity()) {
        this.errors.email = emailInput.validationMessage;
        return;
      }
      this.errors.email = "";
    },
    validatePassword(): void {
      const passwordInput: HTMLInputElement = document.getElementById(
        "password"
      ) as HTMLInputElement;
      if (!passwordInput.checkValidity()) {
        this.errors.password = passwordInput.validationMessage;
        return;
      }
      this.errors.password = "";
      // missing validation logic for password
    },

    redirectToRegister() {
      this.$router.push({ name: "Register" });
    },
  },
  computed: {
    allowLogin() {
      return (
        this.credentials.email &&
        !this.errors.email &&
        this.credentials.password &&
        !this.errors.password
      );
    },
  },
});
</script>

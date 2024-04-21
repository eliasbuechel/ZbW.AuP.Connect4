<template>
  <div class="login-container">
    <img id="logo" src="" alt="r4d4-logo" />
    <div class="login-container">
      <form @submit.prevent="register">
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
          <span v-if="errors.password" class="error">{{
            errors.password
          }}</span>
        </div>
        <span v-if="errors.registration" class="error">{{
          errors.registration
        }}</span>
        <button
          class="button-submit"
          type="submit"
          :disabled="!allowRegistration"
        >
          Register
        </button>
        <button class="button-link" type="button" @click="redirectToLogin">
          Login
        </button>
      </form>
    </div>
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
  registration: String;
}

export default defineComponent({
  name: "RegisterFrom",
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
        registration: "",
      },
    };
  },
  methods: {
    async register() {
      try {
        await this.$axios.post(
          "http://localhost:5000/register",
          this.credentials
        );
        this.errors.registration = "";
        this.$router.push({ name: "Login" });
      } catch (error: any) {
        this.errors.registration = error.message;
        console.log(error);
      }
    },
    async validateEmail() {
      const emailInput: HTMLInputElement = document.getElementById(
        "email"
      ) as HTMLInputElement;
      if (!emailInput.checkValidity()) {
        this.errors.email = emailInput.validationMessage;
        return;
      }

      try {
        await this.$axios.get(
          "http://localhost:5000/Registration/email-taken",
          {
            params: {
              email: this.credentials.email,
            },
          }
        );
        // missing validation logic for email
        this.errors.email = "";
      } catch (error: any) {
        this.errors.email = error.response.data;
      }
    },
    async validatePassword() {
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
    redirectToLogin() {
      this.$router.push({ name: "Login" });
    },
  },
  computed: {
    allowRegistration() {
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

<template>
  <div class="login-container">
    <img id="logo-register" src="@/assets/images/Logo.png" alt="r4d4-logo" />
    <div class="login-container">
      <form @submit.prevent="register">
        <h2>Register</h2>
        <div class="input-field">
          <label for="email">Email</label>
          <input type="email" id="email" v-model="credentials.email" @focusout="validateEmail" required />
          <span v-if="errors.email" class="error">{{ errors.email }}</span>
        </div>
        <div class="input-field">
          <label for="password">Password</label>
          <input type="password" id="password" v-model="credentials.password" @focusout="validatePassword" required />
          <span v-if="errors.password" class="error">{{ errors.password }}</span>
        </div>
        <span v-if="errors.registration" class="error">{{ errors.registration }}</span>
        <button class="button-submit" type="submit" :disabled="!allowRegistration">Register</button>
        <button class="button-link" type="button" @click="redirectToLogin">Login</button>
      </form>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  data() {
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
    async register(): Promise<void> {
      try {
        await this.$axios.post("/account/register", this.credentials);
        this.errors.registration = "";
        this.redirectToLogin();
      } catch (error: any) {
        this.errors.registration = error.message;
        console.error("Error while registering", error);
      }
    },
    async validateEmail(): Promise<void> {
      const emailInput: HTMLInputElement = document.getElementById("email") as HTMLInputElement;
      if (!emailInput.checkValidity()) {
        this.errors.email = emailInput.validationMessage;
        return;
      }

      this.errors.email = "";
    },
    validatePassword(): void {
      const passwordInput: HTMLInputElement = document.getElementById("password") as HTMLInputElement;
      if (!passwordInput.checkValidity()) {
        this.errors.password = passwordInput.validationMessage;
        return;
      }

      this.errors.password = "";
      // missing validation logic for password
    },
    redirectToLogin(): void {
      this.$router.push({ name: "Login" });
    },
  },
  computed: {
    allowRegistration(): boolean {
      let emailIsValid: boolean = this.credentials.email.length != 0 && !this.errors.email;
      let passwordIdValid: boolean = this.credentials.password.length != 0 && !this.errors.password;
      return emailIsValid && passwordIdValid;
    },
  },
});
</script>

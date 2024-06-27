<template>
  <div class="login-container">
    <img id="logo-login" src="@/assets/images/Logo.png" alt="r4d4-logo" />
    <div class="login-container">
      <form @submit.prevent="login">
        <h2>Login</h2>
        <div class="input-field">
          <label for="email">Email</label>
          <input type="email" id="email" v-model="credentials.email" @focusout="validateEmail" required />
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
        <button class="button-submit" type="submit" :disabled="!allowLogin">Login</button>
        <button class="button-link" type="button" @click="redirectToRegister">Registration</button>
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
          login: "",
        },
      };
    },
    methods: {
      async login() {
        try {
          await this.$axios.post("/account/login?useCookies=true", this.credentials, {
            withCredentials: true,
          });

          this.errors.login = "";
          this.$router.push({ name: "Home" });
        } catch (error: any) {
          if (error.response && error.response.status === 401) {
            this.errors.login = "Invalid email or password";
          } else {
            this.errors.login = "An unexpected error occurred. Please try again again.";
          }
        }
      },
      async validateEmail() {
        const emailInput: HTMLInputElement = document.getElementById("email") as HTMLInputElement;
        if (!emailInput.checkValidity()) {
          this.errors.email = emailInput.validationMessage;
          return;
        }
        this.errors.email = "";
      },
      async validatePassword() {
        const passwordInput: HTMLInputElement = document.getElementById("password") as HTMLInputElement;
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
      allowLogin(): boolean {
        let emailIsValid: boolean = this.credentials.email.length != 0 && !this.errors.email;
        let passwordIdValid: boolean = this.credentials.password.length != 0 && !this.errors.password;
        return emailIsValid && passwordIdValid;
      },
    },
  });
</script>

<template>
  <div class="login-container">
    <img id="logo" src="" alt="r4d4-logo" />
    <div class="login-container">
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
          @click="register"
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

<script>
export default {
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
    async register() {
      try {
        await this.$axios.post(
          "http://localhost:5000/register",
          this.credentials
        );
        this.errors.registration = "";
        this.$router.push({ name: "Login" });
      } catch (error) {
        this.errors.registration = error.message;
        console.log(error);
      }
    },
    async validateEmail() {
      const emailInput = document.getElementById("email");
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
      } catch (error) {
        this.errors.email = error.response.data;
      }
    },
    async validatePassword() {
      const passwordInput = document.getElementById("password");
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
};
</script>

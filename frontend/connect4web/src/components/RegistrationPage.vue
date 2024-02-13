<template>
  <div class="page-container">
    <div class="content-container login-content-container">
      <h2 class="page-title">Register</h2>
      <form @submit.prevent="register" class="form">
        <div class="input-field">
          <label for="username">Username</label>
          <input
            type="text"
            id="username"
            v-model="credentials.username"
            required
          />
        </div>
        <div class="input-field">
          <label for="email">Email</label>
          <input type="email" id="email" v-model="credentials.email" required />
        </div>
        <div class="input-field">
          <label for="password">Password</label>
          <input
            type="password"
            id="password"
            v-model="credentials.password"
            required
          />
        </div>
        <label
          v-if="errorMessages.connection.length != 0"
          class="error-message"
          >{{ errorMessages.connection }}</label
        >
        <label
          v-if="
            errorMessages.connection.length == 0 &&
            errorMessages.registration.length != 0
          "
          class="error-message"
          >{{ errorMessages.registration }}</label
        >
        <button
          type="submit"
          class="button-primary first-button"
          :disabled="states.processingRegistration"
        >
          Register
        </button>
      </form>
      <button
        @click="goToLogin"
        class="button-secondary"
        :disabled="signalRNotConnected || states.processingRegistration"
      >
        Login
      </button>
    </div>
  </div>
</template>

<script>
import signalRService from "@/signalRService";
import eventBus from "./../eventBus";

export default {
  data() {
    return {
      credentials: {
        username: "",
        email: "",
        password: "",
      },
      errorMessages: {
        registration: "",
        connection: "",
      },
      states: {
        isConnectedToServer: undefined,
        processingRegistration: false,
      },
    };
  },
  mounted() {
    this.states.isConnectedToServer = signalRService.isConnected;

    eventBus.on("signalr-connected", (isConnected) => {
      this.states.isConnectedToServer = isConnected;
    });

    eventBus.on("signalr-disconnected", (isConnected) => {
      this.states.isConnectedToServer = isConnected;
    });

    signalRService.on("registration-successful", (unverifyedUserJwt) => {
      localStorage.setItem("jwt", unverifyedUserJwt);
      eventBus.emit("active-page-changed", "email-verification");
      this.states.processingRegistration = false;
    });

    signalRService.on("registration-failed", (errorMessage) => {
      this.errorMessages.registration = errorMessage;
      this.states.processingRegistration = false;
    });
  },
  methods: {
    register() {
      this.errorMessages.registration = "";
      this.states.processingRegistration = true;
      signalRService.invoke(
        "Register",
        this.credentials.username,
        this.credentials.email,
        this.credentials.password
      );
      setTimeout(this.processTimeout, 2000);
    },
    goToLogin() {
      eventBus.emit("active-page-changed", "login");
    },
    processTimeout() {
      if (!this.states.processingRegistration) return;

      this.errorMessages.connection = "Not able to process the Request!";
      this.states.processingRegistration = false;
    },
  },
  computed: {
    signalRNotConnected() {
      return !this.states.isConnectedToServer;
    },
  },
  watch: {
    "states.isConnectedToServer": function (newValue) {
      this.errorMessages.connection = newValue
        ? ""
        : "No connection to server!";
    },
  },
};
</script>

<style scoped></style>

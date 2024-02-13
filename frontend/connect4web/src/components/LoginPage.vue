<template>
  <div class="page-container">
    <div class="content-container login-content-container">
      <h2 class="page-title">Login</h2>
      <form @submit.prevent="login" class="form">
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
          v-if="errorMessage.connection.length != 0"
          class="error-message"
          >{{ errorMessage.connection }}</label
        >
        <label
          v-if="
            errorMessage.connection.length == 0 &&
            errorMessage.login.length != 0
          "
          class="error-message"
          >{{ errorMessage.login }}</label
        >
        <button
          type="submit"
          class="button-primary first-button"
          :disabled="signalRNotConnected || state.isProcessingLogin"
        >
          Login
        </button>
      </form>
      <button
        @click="goToRegister"
        class="button-secondary"
        :disabled="state.isProcessingLogin"
      >
        Register
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
        email: "",
        password: "",
      },
      errorMessage: {
        login: "",
        connection: "",
      },
      state: {
        isConnectedToServer: undefined,
        isProcessingLogin: false,
      },
    };
  },
  mounted() {
    this.state.isConnectedToServer = signalRService.isConnected;

    eventBus.on("signalr-connected", (isConnected) => {
      this.state.isConnectedToServer = isConnected;
    });

    eventBus.on("signalr-disconnected", (isConnected) => {
      this.state.isConnectedToServer = isConnected;
    });

    signalRService.client.on("login-successful", (loggedInJwt) => {
      localStorage.setItem("jwt", loggedInJwt);
      eventBus.emit("active-page-changed", "home");
      this.state.isProcessingLogin = false;
    });

    signalRService.client.on(
      "login-succesfull-with-open-email-verification",
      (unverifiedEmailJwt) => {
        localStorage.setItem("jwt", unverifiedEmailJwt);
        eventBus.emit("active-page-changed", "email-verification");
        this.state.isProcessingLogin = false;
      }
    );

    signalRService.client.on("login-failed", (errorMessage) => {
      this.errorMessage.login = errorMessage;
      this.state.isProcessingLogin = false;
    });
  },
  methods: {
    login() {
      this.state.isProcessingLogin = true;
      this.errorMessage.login = "";
      signalRService.client.invoke(
        "Login",
        this.credentials.email,
        this.credentials.password
      );
      this.handleLoginTimeout();
    },
    goToRegister() {
      eventBus.emit("active-page-changed", "registration");
    },
    handleLoginTimeout() {
      setTimeout(() => {
        if (!this.state.isProcessingLogin) return;

        const loginResponseTimeoutErrorMessage =
          "Not able to process the Request!";

        this.errorMessage.connection = loginResponseTimeoutErrorMessage;
        this.state.isProcessingLogin = false;

        setTimeout(() => {
          if (this.errorMessage.connection == loginResponseTimeoutErrorMessage)
            this.errorMessage.connection = "";
        }, 3000);
      }, 5000);
    },
  },
  computed: {
    signalRNotConnected() {
      return !this.state.isConnectedToServer;
    },
  },
  watch: {
    "state.isConnectedToServer": function (newValue) {
      this.errorMessage.connection = newValue ? "" : "No connection to server!";
    },
  },
};
</script>

<style scoped></style>

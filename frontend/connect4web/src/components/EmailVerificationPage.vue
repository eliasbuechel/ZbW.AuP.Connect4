<template>
  <div class="page-container">
    <div class="content-container login-content-container">
      <h2 class="page-title">Email verification</h2>
      <form @submit.prevent="verifyRegistrationCode" class="form">
        <label v-if="userMessage.length != 0" class="user-message">{{
          userMessage
        }}</label>
        <div class="input-field">
          <label for="verification-code">Verification code</label>
          <input
            type="text"
            id="verification-code"
            v-model="verificationCode"
            required
          />
          <button
            class="button-link"
            type="button"
            @click="resendVerificationCode"
            :disabled="processing || userMessage.length != 0"
          >
            Resend verification code
          </button>
        </div>

        <label
          v-if="errorMessages.connection.length != 0"
          class="error-message"
          >{{ errorMessages.connection }}</label
        >
        <label
          v-if="
            errorMessages.connection.length == 0 &&
            errorMessages.emailVerification.length != 0
          "
          class="error-message"
          >{{ errorMessages.emailVerification }}</label
        >
        <button
          type="submit"
          class="button-primary first-button"
          :disabled="signalRNotConnected || processing"
        >
          Verify
        </button>
      </form>
      <button
        @click="goToLogin"
        class="button-secondary"
        :disabled="processing"
      >
        Back to login
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
      verificationCode: "",
      errorMessages: {
        emailVerification: "",
        connection: "",
      },
      userMessage: "",
      signalRConnected: undefined,
      processing: false,
    };
  },
  mounted() {
    this.signalRConnected = signalRService.isConnected;

    eventBus.on("signalr-connected", (isConnected) => {
      this.signalRConnected = isConnected;
    });

    eventBus.on("signalr-disconnected", (isConnected) => {
      this.signalRConnected = isConnected;
    });

    signalRService.client.on(
      "email-verification-successfull",
      (loggedInJwt) => {
        localStorage.setItem("jwt", loggedInJwt);
        eventBus.emit("active-page-changed", "home");
        this.processing = false;
      }
    );

    signalRService.client.on("email-verification-failed", (errorMessage) => {
      this.errorMessages.emailVerification = errorMessage;
      this.processing = false;
    });

    signalRService.client.on("email-verification-code-resent", () => {
      this.userMessage = "Verification code got resent";
      setTimeout(() => (this.userMessage = ""), 5000);
      this.processing = false;
    });
  },
  methods: {
    verifyRegistrationCode() {
      this.errorMessages.emailVerification = "";
      this.processing = true;
      signalRService.client.invoke(
        "CheckEmailVerificationCode",
        localStorage.getItem("jwt"),
        this.verificationCode
      );
      setTimeout(this.processTimeout, 2000);
    },
    resendVerificationCode() {
      this.processing = true;
      signalRService.client.invoke(
        "ResendEmailVerificationCode",
        localStorage.getItem("jwt")
      );
    },
    goToLogin() {
      eventBus.emit("active-page-changed", "login");
    },
    processTimeout() {
      if (!this.processing) return;

      this.errorMessages.connection = "Not able to process the Request!";
      this.processing = false;
    },
  },
  computed: {
    signalRNotConnected() {
      return !this.signalRConnected;
    },
  },
  watch: {
    signalRConnected: function (newValue) {
      this.errorMessages.connection = newValue
        ? ""
        : "No connection to server!";
    },
  },
};
</script>
<style scoped></style>

<template>
  <LoginPage v-if="activePage == 'login'" />
  <RegistrationPage v-if="activePage == 'registration'" />
  <EmailVerificationPage v-if="activePage == 'email-verification'" />
  <HomePage v-if="activePage == 'home'" />
</template>

<script>
import LoginPage from "./components/LoginPage.vue";
import RegistrationPage from "./components/RegistrationPage.vue";
import EmailVerificationPage from "./components/EmailVerificationPage.vue";
import HomePage from "./components/HomePage.vue";
import eventBus from "./eventBus";

export default {
  name: "App",
  data() {
    return {
      activePage: "registration",
    };
  },
  mounted() {
    eventBus.on(
      "active-page-changed",
      (activePage) => (this.activePage = activePage)
    );

    eventBus.on("redirect-to-login", () => (this.activePage = "login"));
  },
  components: {
    LoginPage,
    RegistrationPage,
    HomePage,
    EmailVerificationPage,
  },
};
</script>

<style>
* {
  box-sizing: border-box;
}

body {
  margin: 0px;
  padding: 0px;
}

#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  margin: 0px;
  padding: 0px;
  height: 100vh;
}

.page-container {
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 3rem;
  margin: 0px;
  height: 100%;
}

.content-container {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  padding: 3rem 2rem;
  border-radius: 0.75rem;
}

.login-content-container {
  background-color: #c5c7c888;
}

.page-title {
  font-size: xx-large;
  margin: 0rem 0rem 1rem 0rem;
}

button {
  padding: 0.4rem 1.5rem;
  margin: 0.4rem 0rem;
  font-size: medium;
  border-radius: 1rem;
  border-width: 1px;
  border-color: #343a40;
  background-color: #343a4033;
}

button:hover {
  background-color: #343a4011;
}
button:active {
  background-color: #343a4055;
}
button:active:disabled {
  background-color: #343a4055;
}

.button-primary {
  background-color: #007bffb0;
  color: #ffffff;
  border: none;
}
.button-primary:hover {
  background-color: #007bff;
}
.button-primary:active {
  background-color: #007bffaa;
}
.button-primary:disabled {
  background-color: #007bff55;
}

.button-secondary {
  background-color: #f8f9fab0;
  color: #343a40;
  border: none;
}
.button-secondary:hover {
  background-color: #f8f9fa;
}
.button-secondary:active {
  background-color: #f8f9faaa;
}
.button-secondary:disabled {
  background-color: #f8f9fa55;
}

.button-link {
  padding: 0px;
  margin: 0.2rem;
  color: #007bffb0;
  background-color: transparent;
  border: none;
  font-size: smaller;
}
.button-link:hover {
  color: #007bff;
  background-color: transparent;
}
.button-link:active {
  color: #007bffaa;
  background-color: transparent;
}
.button-link:disabled {
  color: #007bff55;
  background-color: transparent;
}

.first-button {
  margin-top: 1rem;
}

.error-message {
  color: #dc3545;
  font-size: smaller;
}
.user-message {
  color: #343a40;
  font-size: smaller;
}

.form {
  display: flex;
  flex-direction: column;
  align-items: center;
}
.form > .input-field {
  display: flex;
  flex-direction: column;
  margin: 1rem;
}
.form > .input-field > label {
  text-align: left;
  padding-left: 3px;
  font-size: medium;
}
.form > .input-field > input {
  padding: 0.3rem;
  background-color: transparent;
  border-top: none;
  border-left: none;
  border-right: none;
  border-color: #343a4088;
  border-width: 1px;
  width: 230px;
}
.form > .input-field > input:focus {
  border-color: #343a40;
  outline: none;
}
.form > .input-field > button {
  align-self: flex-end;
}
</style>

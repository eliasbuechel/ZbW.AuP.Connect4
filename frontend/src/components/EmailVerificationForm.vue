<template>
  <div class="login-container">
    <img id="logo-email-verification" src="@/assets/images/Logo.png" alt="r4d4-logo" />
    <form>
      <label for="verification-code">Enter verification code:</label>
      <input type="text" id="verification-code" v-model="verificationCode">
      <button class="button-submit" @click="confirmEmail" type="submit">Confirm</button>
      <span v-if="confirmationResult !== null">{{ confirmationResult }}</span>
      <span v-if="error" class="error">{{ error }}</span>

    </form>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  data() {
    return {
      verificationCode: "",
      confirmationResult: null as string | null,
      error: ""
    };
  },
  methods: {
    async confirmEmail() {
      try {
        await this.$axios.post("http://localhost:5000/email-verification", {
          verificationCode: this.verificationCode
        });
        this.confirmationResult = "Email verification successful";
        this.$router.push({ name: "Home" });
      } catch (error: any) {
        this.error = "Email verification failed";
      }
    }
  }
});
</script>
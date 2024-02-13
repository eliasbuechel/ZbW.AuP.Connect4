<template>
  <div class="page-container">
    <div class="content-container">
      <h1 class="page-title">Connect-4</h1>
      <label>Welcome to connect-4</label>
      <button class="button-primary" @click="logout">Logout</button>
    </div>
  </div>
</template>
<script>
import eventBus from "@/eventBus";
import signalRService from "@/signalRService";

export default {
  mounted() {
    signalRService.client.on("logged-out", () => {
      eventBus.emit("set-user-id", undefined);
      eventBus.emit("active-page-changed", "login");
    });
  },
  data() {
    return {};
  },
  methods: {
    logout() {
      signalRService.client.invoke("logout", localStorage.getItem("jwt"));
    },
  },
};
</script>
<style scoped></style>

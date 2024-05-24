<template>

  <body>
    <router-view></router-view>
    <password-popup v-if="showPasswordPopup" @close="closePasswordPopup"></password-popup>
  </body>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import PasswordPopup from "@/components/PasswordPopup.vue";
import eventBus from "./services/eventBus";


export default defineComponent({
  name: "App",
  components: {
    PasswordPopup,
  },
  data() {
    return {
      showPasswordPopup: false,
    };
  },
  mounted() {
    eventBus.on("open-password-popup", this.openPasswordPopup);
  },
  beforeUnmount() {
    eventBus.off("open-password-popup", this.openPasswordPopup);
  },
  methods: {
    openPasswordPopup() {
      this.showPasswordPopup = true;
    },
    closePasswordPopup() {
      this.showPasswordPopup = false;
    },
  }
});
</script>

<style>
@import "@/assets/global.css";
</style>

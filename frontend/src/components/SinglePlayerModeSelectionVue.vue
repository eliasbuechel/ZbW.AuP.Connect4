<template>
  <div class="container">
    <div class="listing-container">
      <h2>Single players</h2>
      <div class="game-mode-container">
        <button class="button-light" @click="requestSinglePlayerMatch" :disabled="hasPendingRequest">Hard</button>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import signalRHub from "@/services/signalRHub";
import { defineComponent } from "vue";

export default defineComponent({
  name: "SinglePlayerModeSelectionVue",
  props: {
    hasPendingRequest: {
      required: true,
      type: Boolean,
    },
  },
  methods: {
    requestSinglePlayerMatch(): void {
      signalRHub.invoke("RequestSinglePlayerMatch");
    },
  },
});
</script>

<style scoped>
.game-mode-container {
  display: flex;
  justify-content: center;
}
</style>

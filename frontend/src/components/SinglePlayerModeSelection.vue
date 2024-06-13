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
import { PropType, defineComponent } from "vue";

export default defineComponent({
  props: {
    hasPendingRequest: {
      required: true,
      type: Object as PropType<boolean>,
    },
  },
  name: "SinglePlayerModeSelection",
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

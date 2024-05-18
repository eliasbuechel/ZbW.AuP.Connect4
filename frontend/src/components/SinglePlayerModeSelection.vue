<template>
  <div class="container">
    <div class="listing-container">
      <h2>Single player mode</h2>
      <div class="mode-selection-container">
        <button class="button-light" @click="matchSinglePlayerGame">easy</button>
        <!-- <button class="button-light">medium</button>
        <button class="button-light">hard</button> -->
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import signalRHub from "@/services/signalRHub";

export default defineComponent({
  name: "SinglePlayerModeSelection",
  methods: {
    matchSinglePlayerGame(): void {
      signalRHub.invoke("MatchAsSinglePlayerGame");
    },
  },
});
</script>

<style scoped>
.mode-selection-container {
  display: flex;
  justify-content: center;
  align-items: center;
}
</style>

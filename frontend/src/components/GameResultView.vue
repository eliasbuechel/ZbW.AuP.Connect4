<template>
  <div>
    <span>{{ resultMessage }}</span>
  </div>
  <button class="button-light" @click="leaveGameResultView">Back home</button>
</template>
<script lang="ts">
import { GameResult } from "@/types/GameResult";
import { PlayerIdentity } from "@/types/PlayerIdentity";
import { PropType, defineComponent } from "vue";

export default defineComponent({
  name: "GameResult",
  props: {
    gameResult: {
      required: true,
      type: Object as PropType<GameResult>,
    },
    identity: {
      required: true,
      type: Object as PropType<PlayerIdentity>,
    },
  },
  methods: {
    leaveGameResultView(): void {
      this.$emit("leave-game-result-view");
    },
  },
  computed: {
    resultMessage(): string {
      if (this.gameResult.winnerId == null) return "Draw!";
      if (this.gameResult.winnerId === this.identity.id) return "You won!";
      return "You lost!";
    },
  },
});
</script>

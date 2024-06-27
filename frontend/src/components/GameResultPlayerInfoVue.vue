<template>
  <div class="player-info">
    <div class="player-info-card">
      <label class="player-info-name">{{ playerName }}</label>
      <label>Total play time: {{ formattedTotalPlayedMoveTime }} s </label>
    </div>
  </div>
</template>

<script lang="ts">
import { PropType, defineComponent } from "vue";
import { PlayerIdentity } from "@/types/PlayerIdentity";
import TimeFormatter from "@/services/timeFormatter";
import { GameResult } from "@/types/GameResult";

export default defineComponent({
  name: "GameResultPlayerInfoVue",
  props: {
    gameResult: {
      required: true,
      type: Object as PropType<GameResult>,
    },
    player: {
      required: true,
      type: Object as PropType<PlayerIdentity>,
    },
    identity: {
      required: true,
      type: Object as PropType<PlayerIdentity>,
    },
  },
  computed: {
    playerName(): string {
      return this.player.id === this.identity.id ? "you" : this.player.username;
    },
    formattedTotalPlayedMoveTime(): string {
      let totalPlayedMoveTime = 0;
      this.gameResult.playedMoves.forEach((move, i) => {
        if (i % 2 === (this.gameResult.startingPlayerId === this.player.id ? 0 : 1))
          totalPlayedMoveTime += move.duration;
      });
      return TimeFormatter.formatAsSeconds(totalPlayedMoveTime, 0);
    },
  },
});
</script>

<style scoped>
@import "@/assets/playerInfo.css";
</style>

<template>
  <div class="player-info">
    <div class="player-info-card">
      <label class="player-info-name">{{ playerName }}</label>
      <label v-if="showCurrentMoveTime"> Current move time: {{ formattedGameTime }}</label>
      <label v-if="gameHasStarted">Total play time: {{ formattedTotalPlayedMoveTime }} s </label>
    </div>
    <div class="player-info-playing-state">{{ playingState }}</div>
  </div>
</template>

<script lang="ts">
import { PropType, defineComponent } from "vue";
import { InGamePlayer } from "@/types/InGamePlayer";
import Game from "@/types/Game";
import { PlayerIdentity } from "@/types/PlayerIdentity";
import TimeFormatter from "@/services/timeFormatter";

export default defineComponent({
  name: "InGamePlayerInfoVue",
  props: {
    game: {
      required: true,
      type: Object as PropType<Game>,
    },
    player: {
      required: true,
      type: Object as PropType<InGamePlayer>,
    },
    identity: {
      required: true,
      type: Object as PropType<PlayerIdentity>,
    },
  },
  data(): {
    moveTimerId?: number;
    playedMoveTime: number;
    totalPlayedMoveTime: number;
  } {
    return {
      moveTimerId: undefined,
      playedMoveTime: 0,
      totalPlayedMoveTime: 0,
    };
  },
  mounted(): void {
    this.startMoveTimer();
  },
  unmounted(): void {
    if (this.moveTimerId == null) return;
    clearInterval(this.moveTimerId);
  },
  methods: {
    startMoveTimer(): void {
      this.moveTimerId = setInterval(() => {
        if (!this.isPlayerActive) return;
        if (this.game.moveStartTime == null) return;
        this.playedMoveTime = Date.now() - this.game.moveStartTime;
        if (this.player.totalPlayTime == null) return;
        this.totalPlayedMoveTime = this.player.totalPlayTime + this.playedMoveTime;
      }, 100);
    },
  },
  computed: {
    isYourTurn(): boolean {
      return this.game.activePlayer().id === this.identity.id;
    },
    playerName(): string {
      if (this.player != null) {
        if (this.player.id == this.identity.id) return "you";
        return this.player.username;
      }
      return "";
    },
    playingState(): string {
      if (this.game == null) return "";
      if (this.player == null) return "";
      if (!this.game.match.player1.hasConfirmedGameStart || !this.game.match.player2.hasConfirmedGameStart) {
        if (this.player.hasConfirmedGameStart) return "";
        return this.player.id === this.identity.id ? "confirm to start the game" : "confirming game start...";
      }
      if (this.game.activePlayerId === this.player.id) {
        if (this.game.placingField != null) return "";
        else if (this.player.id == this.identity.id) return "your turn!";
        return "playing...";
      }
      return "";
    },
    isPlayerInGame(): boolean {
      if (this.game == null) return false;
      return this.identity.id === this.game.match.player1.id || this.identity.id === this.game.match.player2.id;
    },
    isPlayerActive(): boolean {
      if (this.game == null) return false;
      return this.game.activePlayerId === this.player.id;
    },
    gameHasStarted(): boolean {
      if (this.game == null) return false;
      return this.game.match.player1.hasConfirmedGameStart && this.game.match.player2.hasConfirmedGameStart;
    },
    formattedGameTime(): string {
      return TimeFormatter.formatAsSeconds(this.playedMoveTime, 1);
    },
    formattedTotalPlayedMoveTime(): string {
      return TimeFormatter.formatAsSeconds(this.totalPlayedMoveTime, 0);
    },
    showCurrentMoveTime(): boolean {
      return this.gameHasStarted && this.isPlayerActive && this.game.placingField == null;
    },
  },
});
</script>

<style scoped>
@import "@/assets/playerInfo.css";

.player-info-playing-state {
  animation: pulse 2s infinite;
}

@keyframes pulse {
  0%,
  35% {
    opacity: 1;
    color: var(--color-light);
    transform: translateX(0px);
  }
  50% {
    opacity: 0.8;
    color: var(--color-yellow);
    transform: translateX(3px);
  }
  65%,
  100% {
    opacity: 1;
    color: var(--color-light);
    transform: translateX(0px);
  }
}
</style>

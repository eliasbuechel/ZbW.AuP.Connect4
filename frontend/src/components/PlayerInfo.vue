<template>
  <div class="player-info">
    <label>{{ playerName }}</label>
    <label
      class="move-time"
      v-if="isPlayerInGame && gameHasStarted && isPlayerActive && player.id === identity.id"
    >
      Move time: {{ formattedPlayedMoveTime }}</label
    >
    <label class="move-time" v-if="isPlayerInGame && gameHasStarted && isPlayerActive && player.id === identity.id"
      >Move total time: {{ formattedTotalPlayedMoveTime }}
    </label>
    <div class="playing-state">{{ gameState }}</div>
    <button
      v-if="isPlayerInGame && isPlayerActive && gameHasStarted && player.id === identity.id"
      class="button-light"
      @click="quitGame"
    >
      Quit game
    </button>
  </div>
</template>

<script lang="ts">
  import { PropType, defineComponent } from "vue";
  import { InGamePlayer } from "@/types/InGamePlayer";
  import { Game } from "@/types/Game";
  import { PlayerIdentity } from "@/types/PlayerIdentity";

  export default defineComponent({
    props: {
      game: {
        required: true,
        type: Object as PropType<Game | undefined>,
        default: undefined,
      },
      player: {
        required: true,
        type: Object as PropType<InGamePlayer>,
      },
      identity: {
        required: true,
        type: Object as PropType<PlayerIdentity>,
      },
      playedMoveTime: {
        type: Number,
        default: 0,
      },
      totalPlayedMoveTime: {
        type: Number,
        default: 0,
      },
    },
    methods: {
      quitGame(): void {
        this.$emit("quit-game");
      },
    },
    computed: {
      playerName(): string {
        if (this.player != null) {
          if (this.player.id == this.identity.id) return "you";
          return this.player.username;
        }
        return "";
      },
      gameState(): string {
        if (this.game == null) return "";
        if (this.player == null) return "";
        if (!this.player.hasConfirmedGameStart)
          return this.player.id === this.identity.id ? "confirm to start the game" : "confirming game start ...";
        if (this.game.activePlayerId === this.player.id) {
          if (this.player.id == this.identity.id) return "your turn!";
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
      formattedPlayedMoveTime(): string {
        return this.playedMoveTime.toFixed(1) + "s";
      },
      formattedTotalPlayedMoveTime(): string {
        console.log("formattedTotalPlayedMoveTime", this.totalPlayedMoveTime);
        return this.totalPlayedMoveTime.toFixed(1) + "s";
      },
    },
  });
</script>

<style scoped>
  .button-light {
    /* to be in front of the game board */
    z-index: 1;
  }
  .move-time {
    background-color: transparent;
    font-size: 1rem;
  }
</style>

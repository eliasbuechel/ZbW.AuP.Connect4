<template>
  <div class="grid-container">
    <div class="grid-item-page-info-container">
      <h2>Connect Four</h2>
      <h3 v-if="game.gameStartTime != null">{{ formattedGameTime }}</h3>
    </div>
    <div class="grid-item-leave-game-view-button">
      <button v-if="!isGameParticipant" class="button-light" @click="stopWatchingGame">Back home</button>
      <button v-if="isYourTurn || game.isQuittableByEveryone" @click="quitGame" class="button-light">Quit game</button>
    </div>
    <PlayerInfo
      class="grid-item-player1 player-info player-info-left"
      :game="game"
      :identity="identity"
      :player="playerLeft!"
    />
    <PlayerInfo
      class="grid-item-player2 player-info player-info-right"
      :game="game"
      :identity="identity"
      :player="playerRight!"
    />
    <button
      v-if="game != null && !playerLeft?.hasConfirmedGameStart && isGameParticipant"
      class="button-light grid-item-connect4-board confirm-game-start-button"
      @click="confirmGameStart"
    >
      Confirm game start
    </button>
    <Connect4Board
      v-if="game != null && playerLeft?.hasConfirmedGameStart && playerRight?.hasConfirmedGameStart"
      :identity="identity"
      :connect4Board="game.connect4Board"
      :playerLeft="playerLeft!"
      :playerRight="playerRight!"
      :activePlayerId="game.activePlayerId"
      :placingField="game.placingField"
      :lastPlacedStone="game.lastPlacedStone"
      @place-stone="reemitPlaceStone"
      class="grid-item-connect4-board"
    />
  </div>
</template>

<script lang="ts">
import { PropType, defineComponent } from "vue";
import { Game } from "@/types/Game";
import { PlayerIdentity } from "@/types/PlayerIdentity";
import Connect4Board from "./Connect4Board.vue";
import { InGamePlayer } from "@/types/InGamePlayer";
import signalRHub from "@/services/signalRHub";
import PlayerInfoVue from "./PlayerInfoVue.vue";
import TimeFormatter from "@/services/timeFormatter";

export default defineComponent({
  name: "Connect4Game",
  props: {
    game: {
      required: true,
      type: Object as PropType<Game>,
    },
    identity: {
      required: true,
      type: Object as PropType<PlayerIdentity>,
    },
  },
  data(): {
    gameTimerId?: number;
    totalMoveTimerId?: number;
    moveTimerId?: number;
    gameTime: number;
  } {
    return {
      gameTimerId: undefined,
      totalMoveTimerId: undefined,
      moveTimerId: undefined,
      gameTime: 0,
    };
  },
  components: {
    Connect4Board,
    PlayerInfo: PlayerInfoVue,
  },
  mounted(): void {
    this.startGameTimer();
  },
  unmounted(): void {
    if (this.gameTimerId == null) return;
    clearInterval(this.gameTimerId);
  },
  methods: {
    reemitPlaceStone(column: number): void {
      if (this.game.activePlayerId !== this.identity.id) return;
      this.$emit("place-stone", column, this.game.activePlayerId);
    },
    confirmGameStart(): void {
      this.$emit("confirm-game-start");
    },
    quitGame(): void {
      this.$emit("quit-game");
    },
    stopWatchingGame(): void {
      signalRHub.invoke("StopWatchingGame");
      this.$emit("stop-watching-game");
    },
    startGameTimer(): void {
      this.gameTimerId = setInterval(() => {
        if (this.game.gameStartTime == null) return;
        this.gameTime = Date.now() - this.game.gameStartTime;
      }, 100);
    },
  },
  computed: {
    playerLeft(): InGamePlayer {
      return this.game.match.player1.id == this.identity.id ? this.game.match.player1 : this.game.match.player2;
    },
    playerRight(): InGamePlayer | undefined {
      return this.game.match.player1.id == this.identity.id ? this.game.match.player2 : this.game.match.player1;
    },
    namePlayerLeft(): string {
      if (this.playerLeft != null) {
        if (this.playerLeft.id == this.identity.id) return "you";
        return this.playerLeft.username;
      }

      return "";
    },
    namePlayerRight(): string {
      if (this.playerRight != null) {
        if (this.playerRight.id == this.identity.id) return "you";
        return this.playerRight.username;
      }

      return "";
    },
    gameStatePlayerLeft(): string {
      if (this.playerLeft == null) return "";
      if (!this.playerLeft.hasConfirmedGameStart)
        return this.playerLeft.id === this.identity.id ? "confirm to start the game" : "confirming game start ...";
      if (!this.playerRight?.hasConfirmedGameStart) return "";
      if (this.game.activePlayerId === this.playerLeft.id) {
        if (this.playerLeft.id == this.identity.id) return "your turn!";
        return "playing...";
      }
      return "";
    },
    gameStatePlayerRight(): string {
      if (this.playerRight == null) return "";
      if (!this.playerRight.hasConfirmedGameStart)
        return this.playerRight.id === this.identity.id ? "confirm to start the game" : "confirming game start ...";
      if (!this.playerLeft?.hasConfirmedGameStart) return "";
      if (this.game.activePlayerId === this.playerRight.id) {
        if (this.playerRight.id == this.identity.id) return "your turn!";
        return "playing...";
      }
      return "";
    },
    isGameParticipant(): boolean {
      return this.identity.id === this.game.match.player1.id || this.identity.id === this.game.match.player2.id;
    },
    isYourTurn(): boolean {
      return this.identity.id === this.game.activePlayerId;
    },
    isGameStarted(): boolean {
      return this.game.match.player1.hasConfirmedGameStart && this.game.match.player2.hasConfirmedGameStart;
    },
    formattedGameTime(): string {
      return TimeFormatter.formatAsMinutesAndSeconds(this.gameTime);
    },
  },
});
</script>

<style scoped>
.grid-item-page-info-container {
  grid-column: 6 / span 2;
  grid-row: 1 / span 1;
  text-align: center;
}

.grid-item-leave-game-view-button {
  grid-column: 4 / span 6;
  grid-row: 2 / span 1;
  width: fit-content;
  height: fit-content;
  align-self: center;
  justify-self: center;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.grid-item-player1 {
  grid-column: 1 / span 3;
  grid-row: 1 / span 4;
}

.grid-item-player2 {
  grid-column: 10 / span 3;
  grid-row: 1 / span 4;
}

.grid-item-connect4-board {
  grid-column: 1 / span 12;
  grid-row: 3 / span 10;
}

.grid-item-game-result {
  grid-column: 2 / span 10;
  grid-row: 3 / span 10;
}

.confirm-game-start-button {
  height: 3rem;
  padding: 1rem;
  width: fit-content;
  justify-self: center;
  align-self: center;
}
</style>

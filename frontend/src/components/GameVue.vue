<template>
  <div class="game-container">
    <div class="game-header">
      <InGamePlayerInfoVue class="player-info-left" :game="game" :identity="identity" :player="playerLeft!" />
      <div class="game-info-container">
        <h2>Connect Four</h2>
        <button v-if="!isGameParticipant" class="button-light" @click="stopWatchingGame">Back home</button>
        <button v-if="isYourTurn || game.isQuittableByEveryone" @click="quitGame" class="button-light">
          Quit game
        </button>
      </div>
      <InGamePlayerInfoVue class="player-info-right" :game="game" :identity="identity" :player="playerRight!" />
    </div>
    <div class="game-board-container">
      <button
        v-if="game != null && !playerLeft?.hasConfirmedGameStart && isGameParticipant"
        class="button-light confirm-game-start-button"
        @click="confirmGameStart"
      >
        Confirm game start
      </button>
      <div class="game-board">
        <div class="game-board-bar">
          <label v-if="game.gameStartTime != null" class="total-game-time">{{ formattedGameTime }}</label>
          <div v-if="placingField != null" class="playing-move-info-container">
            <div
              :class="{
                loadingCube: true,
                loadingCubeLeft: leftPlayersTurn,
                loadingCubeRight: rightPlayersTurn,
              }"
            ></div>
            <label>placing stone on roboter...</label>
          </div>
          <button
            v-else-if="showHintRequesting"
            class="button-glowing"
            @click="getHint"
            :disabled="!hintRequestEnabled"
          >
            Hints {{ activePlayer.hintsLeft }}/2
          </button>
        </div>
        <BoardVue
          v-if="game != null && playerLeft?.hasConfirmedGameStart && playerRight?.hasConfirmedGameStart"
          :identity="identity"
          :game="game"
          @place-stone="reemitPlaceStone"
        />
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { PropType, defineComponent } from "vue";
import Game from "@/types/Game";
import { PlayerIdentity } from "@/types/PlayerIdentity";
import BoardVue from "./BoardVue.vue";
import { InGamePlayer } from "@/types/InGamePlayer";
import signalRHub from "@/services/signalRHub";
import InGamePlayerInfoVue from "./InGamePlayerInfoVue.vue";
import TimeFormatter from "@/services/timeFormatter";
import { Field } from "@/types/Field";

export default defineComponent({
  name: "GameVue",
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
    isUsingHint: boolean;
  } {
    return {
      gameTimerId: undefined,
      totalMoveTimerId: undefined,
      moveTimerId: undefined,
      gameTime: 0,
      isUsingHint: false,
    };
  },
  components: {
    BoardVue: BoardVue,
    InGamePlayerInfoVue: InGamePlayerInfoVue,
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
      this.isUsingHint = false;
      this.$emit("place-stone", column, this.game.activePlayerId);
    },
    getHint(): void {
      if (!this.isYourTurn) return;
      if (this.placingField != null) return;
      if (this.activePlayer.hintsLeft <= 0) return;
      if (this.activePlayer.currentHint != null) return;
      this.isUsingHint = true;
      signalRHub.invoke("GetHint");
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
      return this.game.playerLeft(this.identity);
    },
    playerRight(): InGamePlayer {
      return this.game.playerRight(this.identity);
    },
    namePlayerLeft(): string {
      return this.playerLeft.id === this.identity.id ? "you" : this.playerLeft.username;
    },
    namePlayerRight(): string {
      return this.playerRight.username;
    },
    gameStatePlayerLeft(): string {
      if (!this.playerLeft.hasConfirmedGameStart)
        return this.playerLeft.id === this.identity.id ? "confirm to start the game" : "confirming game start ...";
      if (!this.playerRight.hasConfirmedGameStart) return "";
      if (this.game.activePlayerId === this.playerLeft.id) {
        if (this.playerLeft.id == this.identity.id) return "your turn!";
        return "playing...";
      }
      return "";
    },
    gameStatePlayerRight(): string {
      if (!this.playerRight.hasConfirmedGameStart)
        return this.playerRight.id === this.identity.id ? "confirm to start the game" : "confirming game start ...";
      if (!this.playerLeft.hasConfirmedGameStart) return "";
      if (this.game.activePlayerId === this.playerRight.id) {
        if (this.playerRight.id == this.identity.id) return "your turn!";
        return "playing...";
      }
      return "";
    },
    isGameParticipant(): boolean {
      return this.game.isParticipant(this.identity);
    },
    isYourTurn(): boolean {
      return this.identity.id === this.game.activePlayerId;
    },
    hasGameStarted(): boolean {
      return this.game.match.player1.hasConfirmedGameStart && this.game.match.player2.hasConfirmedGameStart;
    },
    formattedGameTime(): string {
      return TimeFormatter.formatAsMinutesAndSeconds(this.gameTime);
    },
    placingField(): Field | undefined {
      return this.game.placingField;
    },
    activePlayer(): InGamePlayer {
      return this.game.activePlayer();
    },
    hintRequestEnabled(): boolean {
      return this.activePlayer.currentHint == null && !this.isUsingHint && this.activePlayer.hintsLeft > 0;
    },
    leftPlayersTurn(): boolean {
      return this.playerLeft.id === this.activePlayer.id;
    },
    rightPlayersTurn(): boolean {
      return this.playerRight.id === this.activePlayer.id;
    },
    showHintRequesting(): boolean {
      return this.hasGameStarted && this.isYourTurn && this.placingField == null;
    },
  },
});
</script>

<style scoped>
@import "@/assets/game.css";
</style>

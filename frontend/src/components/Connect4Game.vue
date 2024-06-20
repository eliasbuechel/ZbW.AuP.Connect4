<template>
  <div class="grid-container">
    <div class="grid-item-page-info container">
      <h2>Connect Four</h2>
    </div>
    <button v-if="!youAreInGame" class="button-light grid-item-leave-game-view-button" @click="stopWatchingGame">
      Back home
    </button>
    <div class="grid-item-player1 player-info player-info-left">
      <label>{{ namePlayerLeft }}</label>
      <div class="playing-state">{{ statePlayerLeft }}</div>
      <button v-if="youAreInGame" class="button-light" @click="quitGame">Quit game</button>
    </div>
    <div class="grid-item-player2 player-info player-info-right">
      <label> {{ playerRight.username }}</label>
      <div class="playing-state">{{ statePlyerRight }}</div>
    </div>
    <button
      v-if="youAreInGame && !playerLeft?.hasConfirmedGameStart"
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
      @place-stone="reemitPlaceStone"
      @quit-game="reemitQuitGame"
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

export default defineComponent({
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
  components: {
    Connect4Board,
  },
  methods: {
    reemitPlaceStone(column: number): void {
      if (!this.yourTurn) return;
      this.$emit("place-stone", column);
    },
    confirmGameStart(): void {
      this.$emit("confirm-game-start");
    },
    reemitQuitGame(): void {
      this.$emit("quit-game");
    },
    quitGame(): void {
      this.$emit("quit-game");
    },
    stopWatchingGame(): void {
      signalRHub.invoke("StopWatchingGame");
      this.$emit("stop-watching-game");
    },
  },
  computed: {
    playerLeft(): InGamePlayer {
      return this.game.match.player1.id == this.identity.id ? this.game.match.player1 : this.game.match.player2;
    },
    playerRight(): InGamePlayer {
      return this.game.match.player1.id == this.identity.id ? this.game.match.player2 : this.game.match.player1;
    },
    namePlayerLeft(): string {
      return this.playerLeft.id === this.identity.id ? "you" : this.playerLeft.username;
    },
    statePlayerLeft(): string {
      if (!this.playerLeft.hasConfirmedGameStart)
        return this.playerLeft.id === this.identity.id ? "confirm to start the game" : "confirming game start ...";
      if (!this.playerRight.hasConfirmedGameStart) return "";
      if (this.game.activePlayerId === this.playerLeft.id) {
        if (this.playerLeft.id == this.identity.id) return "your turn!";
        return "playing...";
      }
      return "";
    },
    statePlyerRight(): string {
      if (this.playerRight == null) return "";
      if (!this.playerRight.hasConfirmedGameStart)
        return this.playerRight.id === this.identity.id ? "confirm to start the game" : "confirming game start ...";
      if (!this.playerLeft.hasConfirmedGameStart) return "";
      if (this.game.activePlayerId === this.playerRight.id) {
        if (this.playerRight.id == this.identity.id) return "your turn!";
        return "playing...";
      }
      return "";
    },
    youAreInGame(): boolean {
      return this.playerLeft.id === this.identity.id;
    },
    yourTurn(): boolean {
      return this.game.activePlayerId === this.identity.id;
    },
  },
});
</script>

<style scoped>
.grid-item-page-info {
  grid-column: 4 / span 6;
  grid-row: 1 / span 1;
}
.grid-item-leave-game-view-button {
  grid-column: 4 / span 6;
  grid-row: 2 / span 1;
  width: fit-content;
  height: fit-content;
  align-self: center;
  justify-self: center;
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

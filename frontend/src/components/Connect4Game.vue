<template>
  <div class="grid-container">
    <div class="grid-item-page-info container">
      <h2>Connect Four</h2>
    </div>
    <button
      v-if="!isGameParticipant"
      class="button-light grid-item-leave-game-view-button"
      @click="stopWatchingGame"
    >
      Back home
    </button>

    <PlayerInfo
      class="grid-item-player1 player-info player-info-left"
      :game="game"
      :player="inGamePlayerLeft!"
      :identity="identity"
      @quit-game="quitGame"
    />
    <PlayerInfo
      class="grid-item-player2 player-info player-info-right"
      :game="game"
      :player="inGamePlayerRight!"
      :identity="identity"
      @quit-game="quitGame"
    />
    <button
      v-if="game != null && !inGamePlayerLeft?.hasConfirmedGameStart"
      class="button-light grid-item-connect4-board confirm-game-start-button"
      @click="confirmGameStart"
    >
      Confirm game start
    </button>
    <Connect4Board
      v-if="
        game != null && inGamePlayerLeft?.hasConfirmedGameStart && inGamePlayerRight?.hasConfirmedGameStart
      "
      :identity="identity"
      :connect4Board="game.connect4Board"
      :playerLeft="inGamePlayerLeft!"
      :playerRight="inGamePlayerRight!"
      :activePlayerId="game.activePlayerId"
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
  import PlayerInfo from "./PlayerInfo.vue";
  import { PlayedMove } from "@/types/PlayedMove";

  export default defineComponent({
    props: {
      game: {
        required: true,
        type: Object as PropType<Game | undefined>,
        default: undefined,
      },
      identity: {
        required: true,
        type: Object as PropType<PlayerIdentity>,
      },
      playedMoves: {
        required: true,
        type: Object as PropType<PlayedMove | undefined>,
      },
    },
    components: {
      Connect4Board,
      PlayerInfo,
    },
    methods: {
      reemitPlaceStone(column: number): void {
        if (this.game == null) return;
        if (this.game.activePlayerId !== this.identity.id) return;
        this.$emit("place-stone", column);
      },
      confirmGameStart(): void {
        this.$emit("confirm-game-start");
      },
      reemitQuitGame(): void {
        if (this.game === undefined) return;
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
      inGamePlayerLeft(): InGamePlayer | undefined {
        if (this.game != null)
          return this.game.match.player1.id == this.identity.id
            ? this.game.match.player1
            : this.game.match.player2;

        return undefined;
      },
      inGamePlayerRight(): InGamePlayer | undefined {
        if (this.game != null)
          return this.game.match.player1.id == this.identity.id
            ? this.game.match.player2
            : this.game.match.player1;

        return undefined;
      },
      namePlayerLeft(): string {
        if (this.inGamePlayerLeft != null) {
          if (this.inGamePlayerLeft.id == this.identity.id) return "you";
          return this.inGamePlayerLeft.username;
        }

        return "";
      },
      namePlayerRight(): string {
        if (this.inGamePlayerRight != null) {
          if (this.inGamePlayerRight.id == this.identity.id) return "you";
          return this.inGamePlayerRight.username;
        }

        return "";
      },
      gameStatePlayerLeft(): string {
        if (this.game == null) return "";
        if (this.inGamePlayerLeft == null) return "";
        if (!this.inGamePlayerLeft.hasConfirmedGameStart)
          return this.inGamePlayerLeft.id === this.identity.id
            ? "confirm to start the game"
            : "confirming game start ...";
        if (!this.inGamePlayerRight?.hasConfirmedGameStart) return "";
        if (this.game.activePlayerId === this.inGamePlayerLeft.id) {
          if (this.inGamePlayerLeft.id == this.identity.id) return "your turn!";
          return "playing...";
        }
        return "";
      },
      gameStatePlayerRight(): string {
        if (this.game == null) return "";
        if (this.inGamePlayerRight == null) return "";
        if (!this.inGamePlayerRight.hasConfirmedGameStart)
          return this.inGamePlayerRight.id === this.identity.id
            ? "confirm to start the game"
            : "confirming game start ...";
        if (!this.inGamePlayerLeft?.hasConfirmedGameStart) return "";
        if (this.game.activePlayerId === this.inGamePlayerRight.id) {
          if (this.inGamePlayerRight.id == this.identity.id) return "your turn!";
          return "playing...";
        }
        return "";
      },
      isGameParticipant(): boolean {
        if (this.game == null) return false;
        return (
          this.identity.id === this.game.match.player1.id || this.identity.id === this.game.match.player2.id
        );
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

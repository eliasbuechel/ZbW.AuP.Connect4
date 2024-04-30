<template>
  <div class="grid-container">
    <div class="grid-item-page-info container">
      <h2>Connect Four</h2>
    </div>
    <div class="grid-item-player1 player-info player-info-left">
      <label>{{ player1.username }}</label>
      <button v-if="game != null && game.match.player1.id === identity.id" class="button-light" @click="quitGame">
        Quit game
      </button>
    </div>
    <div class="grid-item-player2 player-info player-info-right">
      <label> {{ player2.username }}</label>
      <button v-if="game != null && game.match.player2.id === identity.id" class="button-light" @click="quitGame">
        Quit game
      </button>
    </div>
    <Connect4Board
      v-if="game != null"
      :connect4Board="game.connect4Board"
      :match="game.match"
      @place-stone="reemitPlaceStone"
      @quit-game="reemitQuitGame"
      class="grid-item-connect4-board"
    />
    <GameResultView
      v-if="gameResult != null"
      :gameResult="gameResult"
      :identity="identity"
      @leave-game-result-view="reemitLeaveGameResultView"
      class="grid-item-game-result"
    />
  </div>
</template>

<script lang="ts">
import { PropType, defineComponent } from "vue";
import { GameResult } from "@/types/GameResult";
import { Game } from "@/types/Game";
import { PlayerIdentity } from "@/types/PlayerIdentity";
import Connect4Board from "./Connect4Board.vue";
import GameResultView from "./GameResultView.vue";

export default defineComponent({
  props: {
    game: {
      required: true,
      type: Object as PropType<Game | undefined>,
      default: undefined,
    },
    gameResult: {
      required: true,
      type: Object as PropType<GameResult | undefined>,
      default: undefined,
    },
    identity: {
      required: true,
      type: Object as PropType<PlayerIdentity>,
    },
  },
  components: {
    Connect4Board,
    GameResultView,
  },
  methods: {
    reemitPlaceStone(column: number): void {
      if (this.game == null) return;
      if (this.game.activePlayerId !== this.identity.id) return;
      this.$emit("place-stone", column);
    },
    reemitQuitGame(): void {
      if (this.game === undefined) return;
      if (this.gameResult !== undefined) return;
      this.$emit("quit-game");
    },
    reemitLeaveGameResultView(): void {
      this.$emit("leave-game-result-view");
    },
    quitGame(): void {
      this.$emit("quit-game");
    },
  },
  computed: {
    resultMessage(): string {
      if (this.gameResult === undefined) return "";
      if (this.identity === undefined) return "";
      if (this.gameResult!.winnerId === undefined) return "Draw!";
      if (this.gameResult!.winnerId === this.identity.id) return "You won!";
      return "You lost!";
    },
    player1(): PlayerIdentity {
      if (!this.game) return { username: "", id: "" };
      return this.game.match.player1;
    },
    player2(): PlayerIdentity {
      if (!this.game) return { username: "", id: "" };
      return this.game.match.player2;
    },
  },
});
</script>

<style scoped>
.grid-item-page-info {
  grid-column: 4 / span 6;
  grid-row: 1 / span 2;
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
  grid-column: 4 / span 6;
  grid-row: 3 / span 10;
}
.grid-item-game-result {
  grid-column: 4 / span 6;
  grid-row: 3 / span 10;
}
</style>

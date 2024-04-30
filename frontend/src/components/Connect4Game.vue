<template>
  <div class="connect4-game">
    <h2>Connect Four</h2>
    <div>
      <label>{{ player1.username }}</label>
    </div>
    <div>
      <label> {{ player2.username }}</label>
    </div>
    <Connect4Board
      v-if="game != null"
      :connect4Board="game.connect4Board"
      :match="game.match"
      @place-stone="reemitPlaceStone"
      @quit-game="reemitQuitGame"
    />
    <GameResultView
      v-if="gameResult != null"
      :gameResult="gameResult"
      :identity="identity"
      @leave-game-result-view="reemitLeaveGameResultView"
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
h2 {
  color: white;
}
</style>

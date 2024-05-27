<template>
  <div class="container">
    <div class="listing-container">
      <h2>Bestlist</h2>
      <ul>
        <li v-for="(gameResult, idx) in bestlist" :key="gameResult.id" class="game-result-entrie">
          <span :class="{ winner: !isDraw(idx), draw: isDraw(idx) }">{{ winner(idx).username }}</span>
          <span> vs. </span>
          <span :class="{ loser: !isDraw(idx), draw: isDraw(idx) }">{{ loser(idx).username }}</span>
          <button class="button-light" @click="showReplay(gameResult)">Replay</button>
        </li>
      </ul>
    </div>
  </div>
</template>

<script lang="ts">
import { GameResult } from "@/types/GameResult";
import { PlayerIdentity } from "@/types/PlayerIdentity";
import { PropType, defineComponent } from "vue";

export default defineComponent({
  name: "BestList",
  props: {
    bestlist: {
      required: true,
      type: Array as PropType<GameResult[]>,
    },
  },
  methods: {
    showReplay(gameResult: GameResult): void {
      this.$emit("show-replay", gameResult);
    },
    winner(idx: number): PlayerIdentity {
      let gameResult: GameResult = this.bestlist[idx];
      if (this.isDraw(idx)) return gameResult.match.player1;

      return gameResult.winnerId === gameResult.match.player1.id ? gameResult.match.player1 : gameResult.match.player2;
    },
    loser(idx: number): PlayerIdentity {
      let gameResult: GameResult = this.bestlist[idx];
      return this.winner(idx).id === gameResult.match.player1.id ? gameResult.match.player2 : gameResult.match.player1;
    },
    isDraw(idx: number): boolean {
      //   let gameResult: GameResult = this.bestlist[idx];
      return this.bestlist[idx].winnerId == null;
    },
  },
});
</script>

<style scoped>
.winner {
  font-weight: bolder;
}

.game-result-entrie {
  display: flex;
  align-items: center;
  flex-direction: row;
}

.game-result-entrie>button {
  margin-left: auto;
}

.game-result-entrie>span {
  margin-right: 0.5rem;
}
</style>

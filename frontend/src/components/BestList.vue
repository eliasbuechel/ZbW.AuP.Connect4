<template>
  <div class="container">
    <div class="listing-container">
      <h2>Bestlist</h2>
      <input class="bestlist-search" v-model="searchTerm" placeholder="Search..." @blur="clearSearch" />

      <div class="header-bar">
        <div>Winner</div>
        <div>Loser</div>
        <div>Winner Time</div>
      </div>

      <ul>
        <li v-for="gameResult in filteredBestlist" :key="gameResult.id" class="game-result-entrie">
          <span
            :class="{ winner: !checkIfGameHasWinningRow(gameResult), draw: checkIfGameHasWinningRow(gameResult) }"
            >{{ showWinner(gameResult).username }}</span
          >
          <span> vs. </span>
          <span
            :class="{ loser: !checkIfGameHasWinningRow(gameResult), draw: checkIfGameHasWinningRow(gameResult) }"
            >{{ ShowLoser(gameResult).username }}</span
          >
          <span>{{ showWinnerTime(gameResult) }}</span>
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
    data(): { searchTerm: string } {
      return {
        searchTerm: "",
      };
    },
    emits: ["show-replay"],
    methods: {
      showReplay(gameResult: GameResult): void {
        let idx = this.bestlist.indexOf(gameResult);
        this.$emit("show-replay", this.bestlist[idx]);
      },
      showWinner(gameResult: GameResult): PlayerIdentity {
        if (this.checkIfGameHasWinningRow(gameResult)) return gameResult.match.player1;
        return gameResult.winnerId === gameResult.match.player1.id
          ? gameResult.match.player1
          : gameResult.match.player2;
      },
      ShowLoser(gameResult: GameResult): PlayerIdentity {
        return this.showWinner(gameResult).id === gameResult.match.player1.id
          ? gameResult.match.player2
          : gameResult.match.player1;
      },
      checkIfGameHasWinningRow(gameResult: GameResult): boolean {
        return gameResult.winnerId === null;
      },
      showWinnerTime(gameResult: GameResult): number {
        let totalDuration = 0;

        if (gameResult.winnerId === gameResult.startingPlayerId) {
          let evenIndexMoves = gameResult.playedMoves.filter((move, index) => index % 2 === 0);
          totalDuration = evenIndexMoves.reduce((total, move) => total + move.duration, 0);
        }

        let oddIndexMoves = gameResult.playedMoves.filter((move, index) => index % 2 !== 0);
        totalDuration = oddIndexMoves.reduce((total, move) => total + move.duration, 0);

        totalDuration = totalDuration / 1000;
        return Math.round(totalDuration * 100) / 100;
      },
      clearSearch() {
        // Timeout to prevent clearing the search term before the click event on the replay button is triggered
        setTimeout(() => {
          this.searchTerm = "";
        }, 1000);
      },
    },
    computed: {
      filteredBestlist(): GameResult[] {
        let bestlist = this.bestlist;

        bestlist.sort((a, b) => this.showWinnerTime(a) - this.showWinnerTime(b));

        if (this.searchTerm !== "") {
          bestlist = bestlist.filter(
            (gameResult) =>
              gameResult.match.player1.username.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
              gameResult.match.player2.username.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
              this.showWinnerTime(gameResult).toString().includes(this.searchTerm)
          );
        }
        return bestlist;
      },
    },
  });
</script>

<style scoped>
  .bestlist-search {
    padding: 0.2rem 0.8rem;
    background-color: transparent;
    border: 2px solid var(--color-orange);
    border-radius: 0.5em;
    font-size: 1rem;
    color: var(--color-light);
  }
  .winner {
    font-weight: bolder;
  }
  .game-result-entrie {
    display: flex;
    align-items: center;
    flex-direction: row;
  }
  .game-result-entrie > button {
    margin-left: auto;
  }
  .game-result-entrie > span {
    margin-right: 0.5rem;
  }
  .header-bar {
    display: flex;
    justify-content: space-between;
    padding: 0.5rem;
    background-color: var(--color-orange);
    color: var(--color-light);
    font-weight: bold;
  }
</style>

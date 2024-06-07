<template>
  <div class="container">
    <div class="listing-container">
      <h2>Bestlist</h2>
      <input class="bestlist-search" v-model="searchTerm" placeholder="Search..." @blur="clearSearch" />
      <ul>
        <li v-for="gameResult in filteredBestlist" :key="gameResult.id" class="game-result-entrie">
          <span :class="{ winner: !isDraw(gameResult), draw: isDraw(gameResult) }">{{
            winner(gameResult).username
          }}</span>
          <span> vs. </span>
          <span :class="{ loser: !isDraw(gameResult), draw: isDraw(gameResult) }">{{
            loser(gameResult).username
          }}</span>
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
    data() {
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
      winner(gameResult: GameResult): PlayerIdentity {
        if (this.isDraw(gameResult)) return gameResult.match.player1;

        return gameResult.winnerId === gameResult.match.player1.id
          ? gameResult.match.player1
          : gameResult.match.player2;
      },
      loser(gameResult: GameResult): PlayerIdentity {
        return this.winner(gameResult).id === gameResult.match.player1.id
          ? gameResult.match.player2
          : gameResult.match.player1;
      },
      isDraw(gameResult: GameResult): boolean {
        //   let gameResult: GameResult = this.bestlist[idx];
        return gameResult.winnerId === null;
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
        if (this.searchTerm === "") return this.bestlist;

        return this.bestlist.filter(
          (gameResult) =>
            gameResult.match.player1.username.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
            gameResult.match.player2.username.toLowerCase().includes(this.searchTerm.toLowerCase())
        );
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
</style>

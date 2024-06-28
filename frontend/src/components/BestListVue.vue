<template>
  <div class="container">
    <div class="listing-container bestList-container">
      <h2>Bestlist</h2>
      <input class="bestlist-search" v-model="searchTerm" placeholder="Search..." @blur="clearSearch" />
      <span v-if="filteredBestlist.length === 0">There are no entries on the leaderboard yet.</span>
      <table class="bestlist-table">
        <thead v-if="filteredBestlist.length > 0">
          <tr>
            <th @click="sortBy('winner')" :class="{ active: sortKey === 'winner' }">
              <div class="header-content">
                <span>Winner</span>
                <div class="sort-arrows">
                  <div class="arrow-up" :class="{ asc: sortKey === 'winner' && sortOrder === 'asc' }"></div>
                  <div class="arrow-down" :class="{ desc: sortKey === 'winner' && sortOrder === 'desc' }"></div>
                </div>
              </div>
            </th>
            <th @click="sortBy('loser')" :class="{ active: sortKey === 'loser' }">
              <div class="header-content">
                <span>Loser</span>
                <div class="sort-arrows">
                  <div class="arrow-up" :class="{ asc: sortKey === 'loser' && sortOrder === 'asc' }"></div>
                  <div class="arrow-down" :class="{ desc: sortKey === 'loser' && sortOrder === 'desc' }"></div>
                </div>
              </div>
            </th>
            <th @click="sortBy('winnerTime')" :class="{ active: sortKey === 'winnerTime' }">
              <div class="header-content">
                <span>Winner Time</span>
                <div class="sort-arrows">
                  <div class="arrow-up" :class="{ asc: sortKey === 'winnerTime' && sortOrder === 'asc' }"></div>
                  <div
                    class="arrow-down"
                    :class="{ desc: sortKey === 'winnerTime' && sortOrder === 'desc' }"
                  ></div>
                </div>
              </div>
            </th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="gameResult in filteredBestlist" :key="gameResult.id">
            <td
              :class="{
                winner: !checkIfGameHasWinningRow(gameResult),
                draw: checkIfGameHasWinningRow(gameResult),
              }"
            >
              {{ showWinner(gameResult).username }}
            </td>
            <td
              :class="{ loser: !checkIfGameHasWinningRow(gameResult), draw: checkIfGameHasWinningRow(gameResult) }"
            >
              {{ showLoser(gameResult).username }}
            </td>
            <td class="winner-time-column">
              {{ showWinnerTime(gameResult).toFixed(2) }}
            </td>
            <td>
              <button class="button-light button-column" @click="showReplay(gameResult)">Replay</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script lang="ts">
  import { GameResult } from "@/types/GameResult";
  import { PlayerIdentity } from "@/types/PlayerIdentity";
  import { PropType, defineComponent } from "vue";

  export default defineComponent({
    name: "BestListVue",
    props: {
      bestlist: {
        required: true,
        type: Array as PropType<GameResult[]>,
      },
    },
    data(): { searchTerm: string; sortKey: string; sortOrder: string } {
      return {
        searchTerm: "",
        sortKey: "winnerTime",
        sortOrder: "asc",
      };
    },
    emits: ["show-replay"],
    methods: {
      sortBy(key: string) {
        if (this.sortKey === key) {
          this.sortOrder = this.sortOrder === "asc" ? "desc" : "asc";
        } else {
          this.sortKey = key;
          this.sortOrder = "asc";
        }
      },

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
      showLoser(gameResult: GameResult): PlayerIdentity {
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
        let bestlist = this.bestlist.slice(); // Copy the array to prevent mutating the original array

        if (this.sortKey === "winner") {
          bestlist.sort((a, b) => {
            let winnerA = this.showWinner(a).username.toLowerCase();
            let winnerB = this.showWinner(b).username.toLowerCase();
            if (this.sortOrder === "asc") {
              return winnerA.localeCompare(winnerB);
            } else {
              return winnerB.localeCompare(winnerA);
            }
          });
        } else if (this.sortKey === "loser") {
          bestlist.sort((a, b) => {
            let loserA = this.showLoser(a).username.toLowerCase();
            let loserB = this.showLoser(b).username.toLowerCase();
            if (this.sortOrder === "asc") {
              return loserA.localeCompare(loserB);
            } else {
              return loserB.localeCompare(loserA);
            }
          });
        } else if (this.sortKey === "winnerTime") {
          bestlist.sort((a, b) => {
            let winnerTimeA = this.showWinnerTime(a);
            let winnerTimeB = this.showWinnerTime(b);
            if (this.sortOrder === "asc") {
              return winnerTimeA - winnerTimeB;
            } else {
              return winnerTimeB - winnerTimeA;
            }
          });
        }

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
  @import "@/assets/bestlist.css";
</style>

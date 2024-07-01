<template>
  <div class="container">
    <div class="listing-container">
      <h2>Bestlist</h2>
      <button class="button-light fixed-top-right" @click="showReplay()" :disabled="selectedGameResult == null">
        Replay
      </button>
      <input class="bestlist-search" v-model="searchTerm" placeholder="Search..." @blur="clearSearch" />
      <span v-if="filteredBestlist.length === 0">There are no entries on the leaderboard yet.</span>
      <div class="scrollable-container">
        <table class="bestlist-table">
          <thead v-if="filteredBestlist.length > 0">
            <tr>
              <th class="winner-time">
                <div
                  class="table-header-column"
                  @click="sortBy('winnerTime')"
                  :class="{ active: sortKey === 'winnerTime' }"
                >
                  <span>Winner Time</span>
                  <SortingDisplayer sortingKey="winnerTime" :currentSortingKey="sortKey" :sortingOrder="sortOrder" />
                </div>
              </th>
              <th>
                <div class="table-header-column" @click="sortBy('winner')" :class="{ active: sortKey === 'winner' }">
                  <span>Winner</span>
                  <SortingDisplayer sortingKey="winner" :currentSortingKey="sortKey" :sortingOrder="sortOrder" />
                </div>
              </th>
              <th>
                <div class="table-header-column" @click="sortBy('loser')" :class="{ active: sortKey === 'loser' }">
                  <span>Loser</span>
                  <SortingDisplayer sortingKey="loser" :currentSortingKey="sortKey" :sortingOrder="sortOrder" />
                </div>
              </th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="gameResult in filteredBestlist"
              :key="gameResult.id"
              :class="{ gameResultRow: true, selected: isSelectedRow(gameResult.id) }"
              @click="selectGameResult(gameResult.id)"
            >
              <td class="winner-time-column">
                {{ showWinnerTime(gameResult).toFixed(2) }}
              </td>
              <td
                :class="{
                  playerName: true,
                  winner: !checkIfGameHasWinningRow(gameResult),
                  draw: checkIfGameHasWinningRow(gameResult),
                }"
              >
                <TextDisplayer :text="showWinner(gameResult).username" :maxCaracters="30" />
              </td>
              <td
                :class="{
                  playerName: true,
                  loser: !checkIfGameHasWinningRow(gameResult),
                  draw: checkIfGameHasWinningRow(gameResult),
                }"
              >
                <TextDisplayer :text="showLoser(gameResult).username" :maxCaracters="30" />
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { GameResult } from "@/types/GameResult";
import { PlayerIdentity } from "@/types/PlayerIdentity";
import { PropType, defineComponent } from "vue";
import SortingDisplayer from "./SortingDisplayer.vue";
import TextDisplayer from "./TextDisplayer.vue";

export default defineComponent({
  name: "BestListVue",
  props: {
    bestlist: {
      required: true,
      type: Array as PropType<GameResult[]>,
    },
  },
  data(): { searchTerm: string; sortKey: string; sortOrder: string; selectedGameResult?: GameResult } {
    return {
      searchTerm: "",
      sortKey: "winnerTime",
      sortOrder: "asc",
      selectedGameResult: undefined,
    };
  },
  emits: ["show-replay"],
  components: {
    SortingDisplayer,
    TextDisplayer,
  },
  methods: {
    sortBy(key: string) {
      if (this.sortKey === key) {
        this.sortOrder = this.sortOrder === "asc" ? "desc" : "asc";
      } else {
        this.sortKey = key;
        this.sortOrder = "asc";
      }
    },
    showReplay(): void {
      if (this.selectedGameResult == null) return;
      this.$emit("show-replay", this.selectedGameResult);
    },
    showWinner(gameResult: GameResult): PlayerIdentity {
      if (this.checkIfGameHasWinningRow(gameResult)) return gameResult.match.player1;
      return gameResult.winnerId === gameResult.match.player1.id ? gameResult.match.player1 : gameResult.match.player2;
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
    selectGameResult(gameResultId: string): void {
      this.bestlist.forEach((x) => {
        if (x.id === gameResultId) this.selectedGameResult = x;
      });
    },
    isSelectedRow(gameResultId: string): boolean {
      if (this.selectedGameResult == null) return false;
      return this.selectedGameResult.id === gameResultId;
    },
    leftBestlist(): void {
      console.log("left bestlist");
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
table {
  border-collapse: collapse;
  border-radius: 0.3rem;
}

table,
th,
td {
  border: none;
}

th,
td {
  padding: 0.5rem;
  text-align: left;
  width: 35%;
}

th:first-child,
td:first-child {
  width: 95px; /* Adjust this value as needed */
}

thead {
  position: sticky;
  top: 0;
  z-index: 1;
  background-color: var(--color-container-bg);
}

.table-header-column {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 0.5rem;
  cursor: pointer;
}

.table-header-column.winner-time {
  max-width: 95px;
}

.playerName {
  max-width: 170px;
  overflow: hidden;
  text-wrap: nowrap;
}

.gameResultRow {
  cursor: pointer;
  border-left: 2px solid transparent;
}

.gameResultRow.selected {
  background-color: var(--color-dark);
  border-color: var(--color-orange);
}

.gameResultRow:hover {
  background-color: var(--color-dark);
}

table {
  width: 100%;
  height: 100%;
  overflow: scroll;
}

.active {
  color: var(--color-orange);
}

.winner-time-column {
  text-align: end;
  padding-right: 2rem;
}

.scrollable-container {
  position: relative;
  overflow-y: scroll;
  max-height: 30vh;
}

.bestlist-search {
  color: var(--color-light);
  background-color: transparent;
  border: 2px solid var(--color-light);
  border-radius: 0.5em;
  padding: 0.2rem 0.8rem;
  font-size: 1rem;
  transition: border-color 0.2s;
}
</style>

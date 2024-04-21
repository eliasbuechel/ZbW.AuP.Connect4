<template>
  <div class="connect4-game">
    <h2>Connect Four</h2>
    <div class="board">
      <div v-for="(column, colIdx) in connect4Board" :key="colIdx" class="column" @click="placeStone(column)">
        <div v-for="(cell, rowIdx) in column" :key="rowIdx" :class="{ cell: true, [cell.color]: cell }"></div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import signalRHub from "@/services/signalRHub";

interface Cell {
  color: string;
}

export default defineComponent({
  mounted() {
    signalRHub.start();
  },
  data(): { currentPlayer: string; connect4Board: Cell[][]; gameOver: boolean } {
    return {
      currentPlayer: "g",
      connect4Board: Array.from({ length: 6 }, () => Array.from({ length: 7 }, () => ({ color: "t" }))),
      gameOver: false,
    };
  },
  methods: {
    placeStone(column: Cell[]): void {
      if (column[column.length - 1].color != "t") return;

      for (let i = 0; i < column.length; i++) {
        let cell = column[i];

        if (cell.color == "t") {
          cell.color = this.currentPlayer;
          this.currentPlayer = this.currentPlayer == "g" ? "b" : "g";
          break;
        }
      }
    },
  },
});
</script>

<style scoped>
h2 {
  color: white;
}

.board {
  display: flex;
  border: 2px solid yellow;
  border-top: none;
}

.column {
  border: 2px solid yellow;
  border-top: none;
  border-bottom: none;
  display: flex;
  flex-direction: column-reverse;
}

.column:hover {
  background-color: #ffffff33;
}

.cell {
  background-color: orange;
  width: 4rem;
  height: 4rem;
  border-radius: 50%;
  margin: 0 0.2rem;
}

.t {
  background-color: transparent;
}

.g {
  background-color: green;
}

.b {
  background-color: blue;
}
</style>

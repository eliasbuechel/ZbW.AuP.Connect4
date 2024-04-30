<template>
  <div class="board">
    <div v-for="(column, colIdx) in connect4Board" :key="colIdx" class="column" @click="placeStone(colIdx)">
      <div
        v-for="(cell, rowIdx) in column"
        :key="rowIdx"
        :class="{ cell: true, g: cell === match.player1.id, b: cell === match.player2.id }"
      ></div>
    </div>
  </div>
  <button class="button-light" @click="quitGame">Quit game</button>
</template>

<script lang="ts">
import { Match } from "@/types/Match";
import { PropType, defineComponent } from "vue";

export default defineComponent({
  name: "Connect4Board",
  props: {
    connect4Board: {
      required: true,
      type: Array as PropType<String[][]>,
    },
    match: {
      required: true,
      type: Object as PropType<Match>,
    },
  },
  methods: {
    placeStone(column: number): void {
      this.$emit("place-stone", column);
    },
    quitGame(): void {
      this.$emit("quit-game");
    },
  },
});
</script>

<style scoped>
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
  background-color: transparent;
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

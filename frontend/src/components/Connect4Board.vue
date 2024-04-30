<template>
  <div>
    <div class="board">
      <div v-for="(column, colIdx) in connect4Board" :key="colIdx" class="column" @click="placeStone(colIdx)">
        <div
          v-for="(cell, rowIdx) in column"
          :key="rowIdx"
          :class="{ cell: true, colorPlayer1: cell === match.player1.id, colorPlayer2: cell === match.player2.id }"
        ></div>
      </div>
    </div>
  </div>
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
  },
});
</script>

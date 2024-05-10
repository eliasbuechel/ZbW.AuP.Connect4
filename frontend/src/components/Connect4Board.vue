<template>
  <div>
    <div class="board">
      <div
        v-for="(column, colIdx) in connect4Board"
        :key="colIdx"
        :class="{ column: true, playableColumn: isYourTurn && !columnIsFull(colIdx) }"
        @click="placeStone(colIdx)"
      >
        <div
          v-for="(cell, rowIdx) in column"
          :key="rowIdx"
          :class="{ cell: true, colorPlayerLeft: cell === playerLeftId, colorPlayerRight: cell === playerRightId }"
        ></div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { PlayerIdentity } from "@/types/PlayerIdentity";
import { PropType, defineComponent } from "vue";

export default defineComponent({
  name: "Connect4Board",
  props: {
    identity: {
      required: true,
      type: Object as PropType<PlayerIdentity>,
    },
    connect4Board: {
      required: true,
      type: Array as PropType<String[][]>,
    },
    playerLeftId: {
      required: true,
      type: Object as PropType<string>,
    },
    playerRightId: {
      required: true,
      type: Object as PropType<string>,
    },
    activePlayerId: {
      required: true,
      type: Object as PropType<string>,
    },
  },
  methods: {
    placeStone(column: number): void {
      if (!this.isYourTurn) return;
      if (this.columnIsFull(column)) return;
      this.$emit("place-stone", column);
    },
    columnIsFull(colIdx: number): boolean {
      return this.connect4Board[colIdx][this.connect4Board[colIdx].length - 1] != "";
    },
  },
  computed: {
    isYourTurn(): boolean {
      return this.activePlayerId === this.identity.id;
    },
  },
});
</script>

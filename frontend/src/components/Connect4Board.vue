<template>
  <div class="game-board-container">
    <button
      v-if="activePlayerId === identity.id"
      class="button-glowing"
      @click="getHint"
      :disabled="activePlayer.currentHint != null || activePlayer.hintsLeft <= 0"
    >
      Hints {{ activePlayer.hintsLeft }}/2
    </button>
    <div class="board">
      <div
        v-for="(column, colIdx) in connect4Board"
        :key="colIdx"
        :class="{ column: true, playableColumn: isYourTurn && !columnIsFull(colIdx), hint: isHint(colIdx) }"
        @click="placeStone(colIdx)"
      >
        <div
          v-for="(cell, rowIdx) in column"
          :key="rowIdx"
          :class="{
            cell: true,
            colorPlayerLeft: cell === playerLeft.id,
            colorPlayerRight: cell === playerRight.id,
            nextPlacingCell: isNextPlacableCell(colIdx, rowIdx),
          }"
        ></div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import signalRHub from "@/services/signalRHub";
import { InGamePlayer } from "@/types/InGamePlayer";
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
    playerLeft: {
      required: true,
      type: Object as PropType<InGamePlayer>,
    },
    playerRight: {
      required: true,
      type: Object as PropType<InGamePlayer>,
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
    getHint(): void {
      if (this.activePlayer.currentHint != null) return;
      signalRHub.invoke("GetHint");
    },
    columnIsFull(colIdx: number): boolean {
      return this.connect4Board[colIdx][this.connect4Board[colIdx].length - 1] != "";
    },
    isHint(colIdx: number): boolean {
      if (this.activePlayer.currentHint == null) return false;
      return this.activePlayer.currentHint == colIdx;
    },
    isNextPlacableCell(colIdx: number, rowIdx: number): boolean {
      if (this.connect4Board[colIdx][rowIdx] != "") return false;
      if (rowIdx <= 0) return true;
      return this.connect4Board[colIdx][rowIdx - 1] != "";
    },
  },
  computed: {
    isYourTurn(): boolean {
      return this.activePlayerId === this.identity.id;
    },
    activePlayer(): InGamePlayer {
      return this.playerLeft.id === this.activePlayerId ? this.playerLeft : this.playerRight;
    },
  },
});
</script>

<style scoped>
.game-board-container {
  display: grid;
  grid-template-rows: 4rem auto;
  flex-direction: column;
  align-items: center;
}

.game-board-container > button {
  justify-self: center;
}

.game-board-container > .board {
  align-self: flex-start;
  grid-row-start: 2;
}
</style>

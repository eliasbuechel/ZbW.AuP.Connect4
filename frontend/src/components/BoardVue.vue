<template>
  <div class="board">
    <div
      v-for="(column, colIdx) in board"
      :key="colIdx"
      :class="{
        column: true,
        playableColumn: isPlayableColumn(colIdx),
        hint: isHint(colIdx),
      }"
      @click="placeStone(colIdx)"
    >
      <div
        v-for="(cell, rowIdx) in column"
        :key="rowIdx"
        :class="{
          cell: true,
          colorPlayerLeft: cell === playerLeft.id || isLeftPlayerPlacingCell(colIdx, rowIdx),
          colorPlayerRight: cell === playerRight.id || isRightPlayerPlacingCell(colIdx, rowIdx),
          nextPlacingCell: isNextPlacableCell(colIdx, rowIdx),
          placingCell: isPlacingCell(colIdx, rowIdx),
          lastPlacedStone: isLastPlacedStone(colIdx, rowIdx) && placingField == null,
        }"
      ></div>
    </div>
  </div>
</template>

<script lang="ts">
import { Field } from "@/types/Field";
import Game from "@/types/Game";
import { InGamePlayer } from "@/types/InGamePlayer";
import { PlayerIdentity } from "@/types/PlayerIdentity";
import { PropType, defineComponent } from "vue";

export default defineComponent({
  name: "BoardVue",
  props: {
    identity: {
      required: true,
      type: Object as PropType<PlayerIdentity>,
    },
    game: {
      required: true,
      type: Object as PropType<Game>,
    },
  },
  methods: {
    placeStone(column: number): void {
      if (!this.isYourTurn) return;
      if (this.columnIsFull(column)) return;
      if (this.placingField != null) return;
      this.$emit("place-stone", column);
    },
    columnIsFull(colIdx: number): boolean {
      return this.game.gameBoard.board[colIdx][this.game.gameBoard.board[colIdx].length - 1] != "";
    },
    isHint(colIdx: number): boolean {
      if (this.activePlayer.currentHint == null) return false;
      if (this.activePlayer.currentHint == colIdx) {
        return true;
      }
      return false;
    },
    isNextPlacableCell(colIdx: number, rowIdx: number): boolean {
      if (this.game.gameBoard.board[colIdx][rowIdx] != "") return false;
      if (rowIdx <= 0) return true;
      return this.game.gameBoard.board[colIdx][rowIdx - 1] != "";
    },
    isPlacingCell(colIdx: number, rowIdx: number): boolean {
      if (this.placingField == null) return false;
      return this.placingField.column === colIdx && this.placingField.row === rowIdx;
    },
    isLeftPlayerCell(colIdx: number, rowIdx: number): boolean {
      return this.board[colIdx][rowIdx] === this.playerLeft.id || this.isLeftPlayerPlacingCell(colIdx, rowIdx);
    },
    isRightPlayerCell(colIdx: number, rowIdx: number): boolean {
      return this.board[colIdx][rowIdx] === this.playerRight.id || this.isRightPlayerPlacingCell(colIdx, rowIdx);
    },
    isLeftPlayerPlacingCell(colIdx: number, rowIdx: number): boolean {
      return this.isPlacingCell(colIdx, rowIdx) && this.playerLeft.id === this.activePlayer.id;
    },
    isRightPlayerPlacingCell(colIdx: number, rowIdx: number): boolean {
      return this.isPlacingCell(colIdx, rowIdx) && this.playerRight.id === this.activePlayer.id;
    },
    isLastPlacedStone(colIdx: number, rowIdx: number): boolean {
      if (this.lastPlacedStone == null) return false;
      return this.lastPlacedStone.column === colIdx && this.lastPlacedStone.row === rowIdx;
    },
    isPlayableColumn(colIdx: number): boolean {
      return this.isYourTurn && this.placingField == null && !this.columnIsFull(colIdx);
    },
  },
  computed: {
    isYourTurn(): boolean {
      return this.activePlayer.id === this.identity.id;
    },
    activePlayer(): InGamePlayer {
      return this.game.activePlayer();
    },
    playerLeft(): InGamePlayer {
      return this.game.playerLeft(this.identity);
    },
    playerRight(): InGamePlayer {
      return this.game.playerRight(this.identity);
    },
    lastPlacedStone(): Field | undefined {
      return this.game.lastPlacedStone;
    },
    placingField(): Field | undefined {
      return this.game.placingField;
    },
    board(): string[][] {
      return this.game.gameBoard.board;
    },
  },
});
</script>

<style scoped></style>

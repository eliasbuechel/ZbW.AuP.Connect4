<template>
  <div class="game-result">
    <div class="game-result-board">
      <div class="game-result-move-navigation">
        <button class="button-light" @click="showPreviousMove" :disabled="moveIndex === 0">&lt;</button>
        <span> {{ moveIndex }} / {{ gameResult.playedMoves.length }} </span>
        <button class="button-light" @click="showNextMove" :disabled="moveIndex === gameResult.playedMoves.length">
          &gt;
        </button>
      </div>
      <div class="board">
        <div v-for="(column, colIdx) in connect4Board" :key="colIdx" class="column">
          <div v-for="(cell, rowIdx) in column" :key="rowIdx" :class="{
          cell: true,
          colorPlayerLeft: cell === playerLeftId,
          colorPlayerRight: cell === playerRightId,
          gameResultCell: true,
        }">
            <div v-if="lastMoveWidthLine && isInLine(colIdx, rowIdx)" class="inLine"></div>
          </div>
        </div>
      </div>
    </div>
    <div class="game-result-message">
      <h3>{{ resultMessage }}</h3>
      <button class="button-light" @click="leaveGameResultView">Back home</button>
    </div>
  </div>
</template>
<script lang="ts">
import { GameResult } from "@/types/GameResult";
import { PlayerIdentity } from "@/types/PlayerIdentity";
import { PropType, defineComponent } from "vue";

interface GameResultState {
  connect4Board: string[][];
  moveIndex: number;
}

export default defineComponent({
  name: "GameResult",
  props: {
    gameResult: {
      required: true,
      type: Object as PropType<GameResult>,
    },
    identity: {
      required: true,
      type: Object as PropType<PlayerIdentity>,
    },
  },
  mounted() {
    for (let i = 0; i < this.connect4Board.length; i++) {
      this.connect4Board[i] = new Array(6);
      for (let j = 0; j < this.connect4Board[i].length; j++) this.connect4Board[i][j] = "";
    }

    while (this.moveIndex < this.gameResult.playedMoves.length) this.showNextMove();
  },
  data(): GameResultState {
    return {
      connect4Board: new Array(7),
      moveIndex: 0,
    };
  },
  methods: {
    showNextMove(): void {
      if (this.moveIndex >= this.gameResult.playedMoves.length) return;
      const column = this.connect4Board[this.gameResult.playedMoves[this.moveIndex].column];
      this.moveIndex++;
      for (let i = 0; i < column.length; i++) {
        if (column[i] == "") {
          column[i] =
            this.moveIndex % 2 === 1
              ? this.gameResult.startingPlayerId === this.playerLeftId
                ? this.playerLeftId
                : this.playerRightId
              : this.gameResult.startingPlayerId === this.playerLeftId
                ? this.playerRightId
                : this.playerLeftId;
          break;
        }
      }
    },
    showPreviousMove(): void {
      if (this.moveIndex <= 0) return;
      const column = this.connect4Board[this.gameResult.playedMoves[this.moveIndex - 1].column];
      this.moveIndex--;
      for (let i = column.length - 1; i >= 0; i--) {
        if (column[i] !== "") {
          column[i] = "";
          return;
        }
      }
    },
    leaveGameResultView(): void {
      this.$emit("leave-game-result-view");
    },
    isInLine(col: number, row: number): boolean {
      if (this.gameResult.line == null) return false;
      for (let i = 0; i < this.gameResult.line.length; i++) {
        if (this.gameResult.line[i].column === col && this.gameResult.line[i].row === row) return true;
      }
      return false;
    },
  },
  computed: {
    resultMessage(): string {
      if (this.gameResult.winnerId == null) return "Draw!";
      if (this.gameResult.winnerId === this.identity.id) return "You won!";
      return "You lost!";
    },
    playerLeftId(): string {
      return this.playerLeft.id;
    },
    playerRightId(): string {
      return this.playerRight.id;
    },
    playerLeft(): PlayerIdentity {
      return this.gameResult.match.player1.id === this.identity.id
        ? this.gameResult.match.player1
        : this.gameResult.match.player2;
    },
    playerRight(): PlayerIdentity {
      return this.gameResult.match.player1.id === this.identity.id
        ? this.gameResult.match.player2
        : this.gameResult.match.player1;
    },
    lastMoveWidthLine(): boolean {
      if (this.gameResult.line == null) return false;
      return this.gameResult.playedMoves.length === this.moveIndex;
    },
  },
});
</script>

<style scoped>
.game-result {
  display: flex;
  flex-direction: row;
}

.game-result-message {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  width: 30%;
}

.game-result-move-navigation {
  margin-bottom: 2vh;
}

.game-result-move-navigation>span {
  margin: 2vw;
}

.game-result-board {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  width: 70%;
}

.gameResultCell {
  display: flex;
  justify-content: center;
  align-items: center;
}

.inLine {
  border: min(2vw, 2vh) solid brown;
  border-radius: min(1vw, 1vh);
}
</style>

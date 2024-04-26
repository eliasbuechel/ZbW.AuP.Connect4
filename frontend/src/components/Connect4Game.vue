<template>
  <div class="connect4-game">
    <h2>Connect Four</h2>
    <div>
      <label>{{ game?.match.player1.username }}</label>
    </div>
    <div>
      <label> {{ game?.match.player2.username }}</label>
    </div>
    <div v-if="gameResult === undefined" class="board">
      <div v-for="(column, colIdx) in game?.connect4Board" :key="colIdx" class="column" @click="placeStone(colIdx)">
        <div
          v-for="(cell, rowIdx) in column"
          :key="rowIdx"
          :class="{ cell: true, g: cell === game?.match.player1.id, b: cell === game?.match.player2.id }"
        ></div>
      </div>
    </div>
    <div v-else>
      <span>{{ resultMessage }}</span>
    </div>
    <button class="button-light" @click="quitGame">Quit game</button>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import signalRHub from "@/services/signalRHub";
import eventBus from "@/services/eventBus";
import { GameResult, PlayerIdentity, GameState, Game } from "@/DataTransferObjects";

export default defineComponent({
  mounted() {
    if (signalRHub.isConnected()) {
      this.subscribe();
      signalRHub.invoke("GetUserData");
      signalRHub.invoke("GetCurrentGame");
    }

    eventBus.on("signalr-connected", this.onSignalRConnected);
    eventBus.on("signalr-disconnected", this.onSignalRDisconnected);
  },
  unmounted() {
    eventBus.off("signalr-connected", this.onSignalRConnected);
    eventBus.off("signalr-disconnected", this.onSignalRDisconnected);

    this.unsubscribe();
  },
  data(): GameState {
    return {
      identity: undefined,
      game: undefined,
      gameResult: undefined,
      isSubscribed: false,
    };
  },
  methods: {
    subscribe(): void {
      if (this.isSubscribed) return;
      signalRHub.on("send-current-game", this.updateGame);
      signalRHub.on("send-user-data", this.updateUserIdentity);
      signalRHub.on("move-played", this.onMovePlayed);
      signalRHub.on("game-ended", this.onGameEnded);
    },
    unsubscribe(): void {
      if (!this.isSubscribed) return;
      signalRHub.on("send-current-game", this.updateGame);
      signalRHub.off("send-user-data", this.updateUserIdentity);
    },
    placeStone(colIdx: number): void {
      if (!this.game) return;
      if (this.identity === undefined) return;
      if (this.identity!.id !== this.game!.activePlayerId) return;
      if (!this.doMove(this.game!.activePlayerId, colIdx)) return;
      signalRHub.invoke("PlayMove", colIdx);
    },
    quitGame(): void {
      eventBus.emit("quit-game");
      if (this.game === undefined) return;
      if (this.gameResult !== undefined) return;
      signalRHub.invoke("QuitGame");
    },
    switchActivePlayer(): void {
      this.game!.activePlayerId =
        this.game!.activePlayerId === this.game!.match.player1.id
          ? this.game!.match.player2.id
          : this.game!.match.player1.id;
    },
    doMove(playerId: string, colIdx: number): boolean {
      if (this.game === undefined) return false;

      let column = this.game!.connect4Board[colIdx];
      if (column[column.length - 1] != "") return false;

      for (let i = 0; i < column.length; i++) {
        if (column[i] == "") {
          column[i] = this.game!.activePlayerId;
          this.switchActivePlayer();
          return true;
        }
      }

      return false;
    },
    updateUserIdentity(identity: PlayerIdentity): void {
      this.identity = identity;
    },
    updateGame(game: Game): void {
      this.game = game;
    },
    onMovePlayed(colIdx: number): void {
      if (!this.game) return;
      if (this.identity === undefined) return;
      this.doMove(this.game!.activePlayerId, colIdx);
    },
    onGameEnded(gameResult: GameResult): void {
      this.gameResult = gameResult;
    },
    onSignalRConnected(): void {
      this.subscribe();
      signalRHub.invoke("GetOnlinePlayers");
      signalRHub.invoke("GetUserData");
    },
    onSignalRDisconnected(): void {
      this.unsubscribe();
    },
  },
  computed: {
    resultMessage(): string {
      if (this.gameResult === undefined) return "";
      if (this.identity === undefined) return "";
      if (this.gameResult!.winnerId === undefined) return "Draw!";
      if (this.gameResult!.winnerId === this.identity.id) return "You won!";
      return "You lost!";
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

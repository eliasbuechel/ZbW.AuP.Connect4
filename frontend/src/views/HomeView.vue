<template>
  <MainBoard v-if="!isInGame" />
  <Connect4Game :state="gameState" v-else />
</template>

<script lang="ts">
import { defineComponent } from "vue";
import signalRHub from "@/services/signalRHub";
import MainBoard from "@/components/MainBoard.vue";
import Connect4Game from "@/components/Connect4Game.vue";
import eventBus from "@/services/eventBus";
import { Game, GameState, PlayerIdentity } from "@/DataTransferObjects";

interface HomeState {
  identity?: PlayerIdentity;
  gameState: GameState;
  isSubscribed: boolean;
}

export default defineComponent({
  mounted(): void {
    if (signalRHub.isConnected()) {
      signalRHub.invoke("HasGameStarted");
      signalRHub.invoke("GetUserData");
    }

    eventBus.on("signalr-connected", this.onSignalRConnected);
    eventBus.on("signalr-disconnected", this.onSignalRDisconnected);

    signalRHub.start();
  },
  unmounted() {
    eventBus.off("signalr-connected", this.onSignalRConnected);
    eventBus.off("signalr-disconnected", this.onSignalRDisconnected);

    signalRHub.stop();
    this.unsubscribe();
  },
  data(): HomeState {
    return {
      identity: undefined,
      gameState: {
        identity: undefined,
        game: undefined,
        gameResult: undefined,
        isSubscribed: false,
      },
      isSubscribed: false,
    };
  },
  methods: {
    subscribe(): void {
      if (this.isSubscribed) return;
      eventBus.on("quit-game", this.onQuitGame);
      signalRHub.on("game-started", this.onGameStarted);
      signalRHub.on("send-current-game", this.updateGame);
      signalRHub.on("send-user-data", this.updateUserIdentity);
    },
    unsubscribe(): void {
      if (!this.isSubscribed) return;
      eventBus.off("quit-game", this.onQuitGame);
      signalRHub.off("game-started", this.onGameStarted);
      signalRHub.off("send-current-game", this.updateGame);
      signalRHub.off("send-user-data", this.updateUserIdentity);
    },
    updateUserIdentity(identity: PlayerIdentity): void {
      this.identity = identity;
    },
    updateGame(game: Game): void {
      this.gameState.game = game;
    },
    onGameStarted(): void {
      signalRHub.invoke("GetUserData");
      signalRHub.invoke("GetCurrentGame");
    },
    onQuitGame(): void {
      this.gameState.game = undefined;
      signalRHub.invoke("GetOnlinePlayers");
    },
    onSignalRConnected(): void {
      this.subscribe();
      signalRHub.invoke("HasGameStarted");
    },
    onSignalRDisconnected(): void {
      this.unsubscribe();
    },
  },
  components: {
    MainBoard,
    Connect4Game,
  },
  computed: {
    isInGame(): boolean {
      if (this.gameState.game === undefined) return false;
      if (this.identity === undefined) return false;
      if (this.gameState.game!.match.player1.id === this.identity!.id) return true;
      if (this.gameState.game!.match.player2.id === this.identity!.id) return true;
      return false;
    },
  },
});
</script>

<style scoped></style>

<template>
  <div v-if="!isInGame">
    <MainBoard />
  </div>
  <div v-else>
    <Connect4Game />
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import signalRHub from "@/services/signalRHub";
import MainBoard from "@/components/MainBoard.vue";
import Connect4Game from "@/components/Connect4Game.vue";
import eventBus from "@/services/eventBus";

export default defineComponent({
  mounted(): void {
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
  data(): { inGame: boolean; isSubscribed: boolean } {
    return {
      inGame: false,
      isSubscribed: false,
    };
  },
  methods: {
    subscribe(): void {
      if (this.isSubscribed) return;
      signalRHub.on("game-started", this.onGameStarted);
    },
    unsubscribe(): void {
      if (!this.isSubscribed) return;
      signalRHub.off("game-started", this.onGameStarted);
    },
    onGameStarted(): void {
      this.inGame = true;
    },
    onSignalRConnected(): void {
      this.subscribe();
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
      return this.inGame;
    },
  },
});
</script>
@/components/DashBoard.vue

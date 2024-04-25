<template>
  <div class="listing-container">
    <h2>Game plan</h2>
    <ul>
      <li v-for="player in gamePlan" :key="player.id">
        <span>{{ player.player1.username }} vs. {{ player.player2.username }}</span>
      </li>
    </ul>
  </div>
</template>

<script lang="ts">
import { Match } from "@/DataTransferObjects";
import eventBus from "@/services/eventBus";
import signalRHub from "@/services/signalRHub";
import { defineComponent } from "vue";

export default defineComponent({
  mounted() {
    if (signalRHub.isConnected()) {
      this.subscribe();
      signalRHub.invoke("GetGamePlan");
    }

    eventBus.on("signalr-connected", this.onSignalRConnected);
    eventBus.on("signalr-disconnected", this.onSignalRDisconnected);
  },
  unmounted() {
    eventBus.off("signalr-connected", this.onSignalRConnected);
    eventBus.off("signalr-disconnected", this.onSignalRDisconnected);

    this.unsubscribe();
  },
  data(): { gamePlan: Set<Match>; isSubscribed: boolean } {
    return {
      gamePlan: new Set<Match>(),
      isSubscribed: false,
    };
  },
  methods: {
    subscribe(): void {
      if (this.isSubscribed) return;
      signalRHub.on("send-game-plan", this.updateGamePlan);
      signalRHub.on("matched", this.onMatched);
      signalRHub.on("player-disconnected", this.onPlayerDisconnected);
    },
    unsubscribe(): void {
      if (!this.isSubscribed) return;
      signalRHub.off("send-game-plan", this.updateGamePlan);
      signalRHub.off("matched", this.onMatched);
      signalRHub.off("player-disconnected", this.onPlayerDisconnected);
    },
    updateGamePlan(gamePlan: Match[]) {
      this.gamePlan.clear();
      gamePlan.forEach((m) => this.gamePlan.add(m));
    },
    onMatched(match: Match): void {
      this.gamePlan.add(match);
    },
    onSignalRConnected(): void {
      this.subscribe();
      signalRHub.invoke("GetGamePlan");
    },
    onSignalRDisconnected(): void {
      this.unsubscribe();
    },
    onPlayerDisconnected(): void {
      signalRHub.invoke("GetGamePlan");
    },
  },
});
</script>

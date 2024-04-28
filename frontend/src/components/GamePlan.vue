<template>
  <div class="listing-container">
    <h2>Game plan</h2>
    <ul>
      <li v-for="(player, idx) in gamePlan" :key="player.id" class="match">
        <span class="player1">{{ player.player1.username }}</span>
        <span v-if="idx == 0" class="battle-icon">&#9876;</span>
        <span v-else class="battle-icon">&#x1f91d;</span>
        <span class="player2">{{ player.player2.username }}</span>
      </li>
    </ul>
  </div>
</template>

<script lang="ts">
import { Match } from "@/DataTransferObjects";
import eventBus from "@/services/eventBus";
import signalRHub from "@/services/signalRHub";
import { defineComponent } from "vue";

interface GamePlanState {
  gamePlan: Set<Match>;
  isSubscribed: boolean;
}

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
  data(): GamePlanState {
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
      signalRHub.on("game-ended", this.onGameEnded);
    },
    unsubscribe(): void {
      if (!this.isSubscribed) return;
      signalRHub.off("send-game-plan", this.updateGamePlan);
      signalRHub.off("matched", this.onMatched);
      signalRHub.off("player-disconnected", this.onPlayerDisconnected);
      signalRHub.off("game-ended", this.onGameEnded);
    },
    updateGamePlan(gamePlan: Match[]) {
      this.gamePlan.clear();
      gamePlan.forEach((m) => this.gamePlan.add(m));
    },
    onMatched(match: Match): void {
      this.gamePlan.add(match);
    },
    onGameEnded(): void {
      let match: Match = [...this.gamePlan][0];
      this.gamePlan.delete(match);
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

<style scoped>
.match {
  display: flex;
  color: whitesmoke;
  align-items: center;
  justify-content: stretch;
}
.match > span {
  display: block;
}

.match > .battle-icon {
  font-size: xx-large;
  color: brown;
  flex-grow: 1;
}
match > .player1 {
  text-align: end;
  width: 200px;
}
match > .player2 {
}
</style>

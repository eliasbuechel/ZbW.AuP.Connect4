<template>
  <div class="listing-container">
    <h2>Online players</h2>
    <span class="error">{{ errors.players }}</span>
    <ul>
      <li v-for="player in players" :key="player.id">
        <span>{{ player.username }}</span>
        <button v-if="!player.requestedMatch" class="button-light" @click="requestGame(player)">Request Game</button>
        <button v-if="player.requestedMatch" class="button-light" @click="acceptMatching(player)">Accept match</button>
      </li>
    </ul>
    <button class="button-light" @click="reload">Reload</button>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import signalRHub from "@/services/signalRHub";
import eventBus from "@/services/eventBus";

interface OnlinePlayer {
  id: string;
  username: string;
  requestedMatch: boolean;
}

export default defineComponent({
  mounted() {
    if (!signalRHub.isConnected()) eventBus.on("signalr-connected", this.onSignalRConnected);

    signalRHub.on("get-online-players", this.onUdatePlayers);
    signalRHub.on("player-connected", this.onPlayerConnected);
    signalRHub.on("player-disconnected", this.onPlayerDisconnected);
  },
  unmounted() {
    signalRHub.off("get-online-players", this.onUdatePlayers);
    signalRHub.off("player-connected", this.onPlayerConnected);
    signalRHub.off("player-disconnected", this.onPlayerDisconnected);
  },
  data(): { players: Set<OnlinePlayer>; errors: { players: string } } {
    return {
      players: new Set<OnlinePlayer>(),
      errors: { players: "" },
    };
  },
  methods: {
    async requestGame(player: OnlinePlayer): Promise<void> {
      console.log("Requesting game with player " + player.id);
      player.requestedMatch = true;
    },
    async acceptMatching(player: OnlinePlayer): Promise<void> {
      console.log("Accepted matching with player " + player.id);
      player.requestedMatch = false;
    },
    reload(): void {
      signalRHub.invoke("GetPlayers");
    },
    onUdatePlayers(players: OnlinePlayer[]): void {
      this.players.clear();
      players.forEach((p) => this.players.add(p));
    },
    onPlayerConnected(player: OnlinePlayer): void {
      this.players.add(player);
    },
    onPlayerDisconnected(playerId: string): void {
      this.players.forEach((player) => {
        if (player.id === playerId) {
          this.players.delete(player);
          return;
        }
      });
    },
    onSignalRConnected(): void {
      signalRHub.invoke("GetPlayers");
      eventBus.off("signalr-connected", this.onSignalRConnected);
    },
  },
});
</script>

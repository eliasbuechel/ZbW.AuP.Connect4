<template>
  <div class="listing-container">
    <h2>Online players</h2>
    <span v-if="errors.players" class="error">{{ errors.players }}</span>
    <ul v-else>
      <li v-for="player in players" :key="player.Id">
        <span>{{ player.Username }}</span>
        <button v-if="!player.RequestedMatch" class="button-light" @click="requestGame(player)">Request Game</button>
        <button v-if="player.RequestedMatch" class="button-light" @click="acceptMatching(player)">Accept match</button>
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
  Id: string;
  Username: string;
  RequestedMatch: boolean;
}

export default defineComponent({
  mounted() {
    if (!signalRHub.isConnected()) eventBus.on("signalr-connected", this.onSignalRConnected);

    signalRHub.on("send-online-players", this.onUdatePlayers);
    signalRHub.on("player-connected", this.onPlayerConnected);
    signalRHub.on("player-disconnected", this.onPlayerDisconnected);
  },
  unmounted() {
    signalRHub.off("send-online-players", this.onUdatePlayers);
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
      console.log("Requesting game with player " + player.Id);
      player.RequestedMatch = true;
    },
    async acceptMatching(player: OnlinePlayer): Promise<void> {
      console.log("Accepted matching with player " + player.Id);
      player.RequestedMatch = false;
    },
    reload(): void {
      signalRHub.invoke("GetOnlinePlayers");
    },
    onUdatePlayers(players: OnlinePlayer[]): void {
      console.log(players);
      this.players.clear();
      players.forEach((p) => this.players.add(p));
      console.log(this.players);
    },
    onPlayerConnected(player: OnlinePlayer): void {
      console.log(player);
      this.players.add(player);
    },
    onPlayerDisconnected(playerId: string): void {
      console.log(playerId);
      this.players.forEach((player) => {
        if (player.Id === playerId) {
          this.players.delete(player);
          return;
        }
      });
    },
    onSignalRConnected(): void {
      signalRHub.invoke("GetOnlinePlayers");
      eventBus.off("signalr-connected", this.onSignalRConnected);
    },
  },
});
</script>

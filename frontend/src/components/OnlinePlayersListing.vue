<template>
  <div class="listing-container">
    <h2>Online players</h2>
    <span v-if="errors.players" class="error">{{ errors.players }}</span>
    <ul v-else>
      <li v-for="player in players" :key="player.id">
        <span>{{ player.username }}</span>
        <span v-if="player.matched">Matched</span>
        <span v-else-if="player.youRequestedMatch">Matching pending...</span>
        <div v-else-if="player.requestedMatch">
          <button class="button-light" @click="acceptMatch(player)">Accept match</button>
          <button class="button-light" @click="rejectMatch(player)">Reject match</button>
        </div>
        <button v-else class="button-light" @click="requestMatch(player)" :disabled="hasPendingRequest">
          Request Game
        </button>
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
  youRequestedMatch: boolean;
  matched: boolean;
}

export default defineComponent({
  mounted() {
    if (signalRHub.isConnected()) {
      this.subscribe();
      signalRHub.invoke("GetOnlinePlayers");
    }

    eventBus.on("signalr-connected", this.onSignalRConnected);
    eventBus.on("signalr-disconnected", this.onSignalRDisconnected);
  },
  unmounted() {
    eventBus.off("signalr-connected", this.onSignalRConnected);
    eventBus.off("signalr-disconnected", this.onSignalRDisconnected);

    this.unsubscribe();
  },
  data(): { players: Set<OnlinePlayer>; errors: { players: string }; isSubscribed: boolean } {
    return {
      players: new Set<OnlinePlayer>(),
      errors: { players: "" },
      isSubscribed: false,
    };
  },
  methods: {
    subscribe(): void {
      if (this.isSubscribed) return;
      signalRHub.on("send-online-players", this.onUdatePlayers);
      signalRHub.on("player-connected", this.onPlayerConnected);
      signalRHub.on("player-disconnected", this.onPlayerDisconnected);
      signalRHub.on("player-requested-match", this.onPlayerRequestedMatch);
      signalRHub.on("player-rejected-match", this.onPlayerRejectedMatch);
      signalRHub.on("matched", this.onMatched);
    },
    unsubscribe(): void {
      if (!this.isSubscribed) return;
      signalRHub.off("send-online-players", this.onUdatePlayers);
      signalRHub.off("player-connected", this.onPlayerConnected);
      signalRHub.off("player-disconnected", this.onPlayerDisconnected);
      signalRHub.off("player-requested-match", this.onPlayerRequestedMatch);
      signalRHub.off("player-rejected-match", this.onPlayerRejectedMatch);
      signalRHub.off("matched", this.onMatched);
    },
    requestMatch(player: OnlinePlayer): void {
      signalRHub.invoke("RequestMatch", player.id);
      player.youRequestedMatch = true;
    },
    acceptMatch(player: OnlinePlayer): void {
      signalRHub.invoke("AcceptMatch", player.id);
    },
    rejectMatch(player: OnlinePlayer): void {
      signalRHub.invoke("RejectMatch", player.id);
      player.requestedMatch = false;
    },
    reload(): void {
      console.log("Manual reload...");
      signalRHub.invoke("GetOnlinePlayers");
    },
    onUdatePlayers(players: OnlinePlayer[]): void {
      this.players.clear();
      players.forEach((p) => this.players.add(p));
    },
    onPlayerConnected(player: OnlinePlayer): void {
      console.log(player);
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
    onPlayerRequestedMatch(playerId: string): void {
      this.players.forEach((p) => {
        if (p.id === playerId) {
          p.requestedMatch = true;
          return;
        }
      });
    },
    onPlayerRejectedMatch(playerId: string): void {
      this.players.forEach((p) => {
        if (p.id === playerId) {
          p.youRequestedMatch = false;
          return;
        }
      });
    },
    onMatched(playerId: string) {
      this.players.forEach((p) => {
        if (p.id == playerId) {
          p.matched = true;
          p.requestedMatch = false;
          p.youRequestedMatch = false;
          return;
        }
      });
    },
    onSignalRConnected(): void {
      this.subscribe();
      signalRHub.invoke("GetOnlinePlayers");
    },
    onSignalRDisconnected(): void {
      this.unsubscribe();
    },
  },
  computed: {
    hasPendingRequest(): boolean {
      let doesHavePendingRequest: boolean = false;
      this.players.forEach((p) => {
        if (p.youRequestedMatch) doesHavePendingRequest = true;
      });
      return doesHavePendingRequest;
    },
  },
});
</script>

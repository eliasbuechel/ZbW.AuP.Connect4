<template>
  <div class="listing-container">
    <h2>Online players</h2>
    <ul>
      <li v-for="player in onlinePlayers" :key="player.id" class="matchable-player">
        <span class="matchable-player-username">{{ player.username }}</span>
        <span v-if="player.matched">Matched</span>
        <span v-if="player.youRequestedMatch">Matching pending...</span>
        <button
          v-if="player.requestedMatch && !player.youRequestedMatch"
          class="button-accept"
          @click="acceptMatch(player)"
        >
          &check;
        </button>
        <button
          v-if="player.requestedMatch && !player.youRequestedMatch"
          class="button-danger"
          @click="rejectMatch(player)"
        >
          &cross;
        </button>
        <button
          v-if="!player.matched && !player.youRequestedMatch && !player.requestedMatch"
          class="button-light"
          @click="requestMatch(player)"
          :disabled="hasPendingRequest"
        >
          Request
        </button>
      </li>
    </ul>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import signalRHub from "@/services/signalRHub";
import eventBus from "@/services/eventBus";
import { Match, OnlinePlayer, PlayerIdentity } from "@/DataTransferObjects";

interface PlayerListingState {
  identity?: PlayerIdentity;
  onlinePlayers: Set<OnlinePlayer>;
  isSubscribed: boolean;
}

export default defineComponent({
  mounted() {
    if (signalRHub.isConnected()) {
      this.subscribe();
      signalRHub.invoke("GetOnlinePlayers");
      signalRHub.invoke("GetUserData");
    }

    eventBus.on("signalr-connected", this.onSignalRConnected);
    eventBus.on("signalr-disconnected", this.onSignalRDisconnected);
  },
  unmounted() {
    eventBus.off("signalr-connected", this.onSignalRConnected);
    eventBus.off("signalr-disconnected", this.onSignalRDisconnected);

    this.unsubscribe();
  },
  data(): PlayerListingState {
    return {
      identity: undefined,
      onlinePlayers: new Set<OnlinePlayer>(),
      isSubscribed: false,
    };
  },
  methods: {
    subscribe(): void {
      if (this.isSubscribed) return;
      signalRHub.on("send-online-players", this.onUdateOnlinePlayers);
      signalRHub.on("player-connected", this.onPlayerConnected);
      signalRHub.on("player-disconnected", this.onPlayerDisconnected);
      signalRHub.on("player-requested-match", this.onPlayerRequestedMatch);
      signalRHub.on("you-requested-match", this.onYouRequestedMatch);
      signalRHub.on("player-rejected-match", this.onPlayerRejectedMatch);
      signalRHub.on("you-rejected-match", this.onYouRejectedMatch);
      signalRHub.on("matched", this.onMatched);
      signalRHub.on("send-user-data", this.updateUserIdentity);
    },
    unsubscribe(): void {
      if (!this.isSubscribed) return;
      signalRHub.off("send-online-players", this.onUdateOnlinePlayers);
      signalRHub.off("player-connected", this.onPlayerConnected);
      signalRHub.off("player-disconnected", this.onPlayerDisconnected);
      signalRHub.off("player-requested-match", this.onPlayerRequestedMatch);
      signalRHub.off("you-requested-match", this.onYouRequestedMatch);
      signalRHub.off("player-rejected-match", this.onPlayerRejectedMatch);
      signalRHub.off("you-rejected-match", this.onYouRejectedMatch);
      signalRHub.off("matched", this.onMatched);
      signalRHub.off("send-user-data", this.updateUserIdentity);
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
    onUdateOnlinePlayers(onlinePlayers: OnlinePlayer[]): void {
      this.onlinePlayers.clear();
      onlinePlayers.forEach((p) => this.onlinePlayers.add(p));
    },
    updateUserIdentity(identity: PlayerIdentity): void {
      this.identity = identity;
    },
    onPlayerConnected(player: OnlinePlayer): void {
      console.log(player);
      this.onlinePlayers.add(player);
    },
    onPlayerDisconnected(playerId: string): void {
      this.onlinePlayers.forEach((player) => {
        if (player.id === playerId) {
          this.onlinePlayers.delete(player);
          return;
        }
      });
    },
    onPlayerRequestedMatch(playerId: string): void {
      this.onlinePlayers.forEach((p) => {
        if (p.id === playerId) {
          p.requestedMatch = true;
          return;
        }
      });
    },
    onYouRequestedMatch(playerId: string): void {
      this.onlinePlayers.forEach((p) => {
        if (p.id === playerId) {
          p.youRequestedMatch = true;
          return;
        }
      });
    },
    onPlayerRejectedMatch(playerId: string): void {
      this.onlinePlayers.forEach((p) => {
        if (p.id === playerId) {
          p.youRequestedMatch = false;
          return;
        }
      });
    },
    onYouRejectedMatch(playerId: string): void {
      this.onlinePlayers.forEach((p) => {
        if (p.id === playerId) {
          p.requestedMatch = false;
          return;
        }
      });
    },
    onMatched(match: Match) {
      if (this.identity === undefined) return;
      this.onlinePlayers.forEach((p) => {
        if (
          (p.id === match.player1.id && this.identity?.id === match.player2.id) ||
          (p.id === match.player2.id && this.identity?.id === match.player1.id)
        ) {
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
      signalRHub.invoke("GetUserData");
    },
    onSignalRDisconnected(): void {
      this.unsubscribe();
    },
  },
  computed: {
    hasPendingRequest(): boolean {
      let doesHavePendingRequest: boolean = false;
      this.onlinePlayers.forEach((p) => {
        if (p.youRequestedMatch) doesHavePendingRequest = true;
      });
      return doesHavePendingRequest;
    },
  },
});
</script>

<style scoped>
.matchable-player {
  display: flex;
  align-items: center;
  color: whitesmoke;
}

.matchable-player > .matchable-player-username {
  flex-grow: 1;
}
</style>

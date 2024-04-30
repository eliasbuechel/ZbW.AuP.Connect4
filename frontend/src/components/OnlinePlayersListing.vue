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
import { PropType, defineComponent } from "vue";
import signalRHub from "@/services/signalRHub";
import { PlayerIdentity } from "@/types/PlayerIdentity";
import { OnlinePlayer } from "@/types/OnlinePlayer";

export default defineComponent({
  props: {
    onlinePlayers: {
      required: true,
      type: Array as PropType<OnlinePlayer[]>,
    },
    identity: {
      required: true,
      type: Object as PropType<PlayerIdentity>,
    },
  },
  methods: {
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
@/types/DataTransferObjects

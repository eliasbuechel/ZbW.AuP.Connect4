<template>
  <div class="container">
    <div class="listing-container">
      <h2>Online players</h2>
      <ul>
        <li v-for="player in onlinePlayers" :key="player.id" class="matchable-player">
          <span class="matchable-player-username">{{ player.username }}</span>
          <svg
            v-if="player.matched"
            xmlns="http://www.w3.org/2000/svg"
            width="24"
            height="24"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="4"
            stroke-linecap="round"
            stroke-linejoin="round"
            class="checkmark"
          >
            <polyline points="20 6 9 17 4 12"></polyline>
          </svg>
          <div v-if="player.youRequestedMatch" class="loading-state">
            <div class="loading"></div>
          </div>
          <button
            v-if="player.requestedMatch && !player.youRequestedMatch"
            class="button-accept"
            @click="acceptMatch(player)"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              width="24"
              height="24"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              stroke-width="4"
              stroke-linecap="round"
              stroke-linejoin="round"
              class="checkmark"
            >
              <polyline points="20 6 9 17 4 12"></polyline>
            </svg>
          </button>
          <button
            v-if="player.requestedMatch && !player.youRequestedMatch"
            class="button-danger reject-match-button"
            @click="rejectMatch(player)"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              width="24"
              height="24"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              stroke-width="4"
              stroke-linecap="round"
              stroke-linejoin="round"
              class="cross"
            >
              <line x1="18" y1="6" x2="6" y2="18"></line>
              <line x1="6" y1="6" x2="18" y2="18"></line>
            </svg>
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
    hasPendingRequest: {
      required: true,
      type: Boolean,
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

.reject-match-button {
  margin-left: 0.5rem;
}
</style>
@/types/DataTransferObjects

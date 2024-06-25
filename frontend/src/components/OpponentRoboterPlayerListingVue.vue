<template>
  <div class="container">
    <div class="listing-container">
      <h2>Opponent roboter players</h2>
      <div class="adding-opponent-roboter-player-container">
        <div class="input-field input-field-light">
          <label for="addingHubUrl">Hub url</label>
          <input type="text" id="addingHubUrl" v-model="addingHubUrl" @change="validateAddingHubUrl" required />
          <span v-if="errors.addingHubUrl" class="error">{{ errors.addingHubUrl }}</span>
        </div>
        <button class="button-accept" @click="connect" :disabled="errors.addingHubUrl !== ''">+</button>
      </div>
      <ul>
        <li v-for="player in connectedOpponentRoboterPlayers" :key="player.id" class="matchable-player">
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
import eventBus from "@/services/eventBus";

export default defineComponent({
  name: "OpponentRoboterPlayerListingVue",
  props: {
    connectedOpponentRoboterPlayers: {
      required: true,
      type: Array as PropType<OnlinePlayer[]>,
    },
    identity: {
      required: true,
      type: Object as PropType<PlayerIdentity>,
    },
  },
  data(): { addingHubUrl: string; errors: { addingHubUrl: string } } {
    return {
      addingHubUrl: "",
      errors: {
        addingHubUrl: " ",
      },
    };
  },
  mounted() {
    eventBus.on(
      "NotAbleToConnectToOpponentRoboterPlayer",
      (errorMessage: string) => (this.errors.addingHubUrl = errorMessage)
    );
  },
  methods: {
    connect() {
      signalRHub.invoke("ConnectToOpponentRoboterPlayer", this.addingHubUrl);
    },
    async validateAddingHubUrl() {
      const emailInput: HTMLInputElement = document.getElementById("addingHubUrl") as HTMLInputElement;
      if (!emailInput.checkValidity()) {
        this.errors.addingHubUrl = emailInput.validationMessage;
        return;
      }
      this.errors.addingHubUrl = "";
    },
    requestMatch(player: OnlinePlayer): void {
      signalRHub.invoke("RequestOppoenntRoboterPlyerMatch", player.id);
      player.youRequestedMatch = true;
    },
    acceptMatch(player: OnlinePlayer): void {
      signalRHub.invoke("AcceptOppoenntRoboterPlyerMatch", player.id);
    },
    rejectMatch(player: OnlinePlayer): void {
      signalRHub.invoke("RejectOppoenntRoboterPlyerMatch", player.id);
      player.requestedMatch = false;
    },
  },
  computed: {
    hasPendingRequest(): boolean {
      let doesHavePendingRequest: boolean = false;
      this.connectedOpponentRoboterPlayers.forEach((p) => {
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

.reject-match-button {
  margin-left: 0.5rem;
}

.adding-opponent-roboter-player-container {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.adding-opponent-roboter-player-container > div {
  flex-direction: column;
  width: 100%;
}
.adding-opponent-roboter-player-container > div > input,
.adding-opponent-roboter-player-container > div > span {
  width: 100%;
}
</style>
@/types/DataTransferObjects

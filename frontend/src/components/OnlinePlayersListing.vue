<template>
  <div class="listing-container">
    <h2>Online players</h2>
    <ul>
      <span v-if="errors.players" class="error">{{ errors.players }}</span>
      <li v-else v-for="player in players" :key="player.id">
        {{ player.email }}
        <button
          v-if="!player.requestedMatch"
          class="button-light"
          @click="requestGame(player)"
        >
          Request Game
        </button>
        <button
          v-if="player.requestedMatch"
          class="button-light"
          @click="acceptMatching(player)"
        >
          Accept match
        </button>
      </li>
    </ul>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import signalRHub from "@/services/signalRHub";

interface Player {
  id: string;
  email: string;
  requestedMatch: boolean;
}

interface Errors {
  players: string;
}

export default defineComponent({
  name: "OnlinePlayerListing",
  components: {},
  async mounted(): Promise<void> {
    try {
      signalRHub.invoke("GetPlayers");
    } catch {
      this.errors.players = "Not able to load players.";
    }
  },
  data(): { players: Player[]; errors: Errors } {
    return {
      players: [
        { id: "1", email: "Player 1", requestedMatch: false },
        { id: "2", email: "Player 2", requestedMatch: false },
        { id: "3", email: "Player 3", requestedMatch: false },
      ],
      errors: { players: "" },
    };
  },
  methods: {
    async requestGame(player: Player): Promise<void> {
      console.log("Requesting game with player " + player.id);
      player.requestedMatch = true;
    },
    async acceptMatching(player: Player): Promise<void> {
      console.log("Accepted matching with player " + player.id);
      player.requestedMatch = false;
    },
  },
});
</script>

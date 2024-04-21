<template>
  <div class="listing-container">
    <h2>Online players</h2>
    <ul>
      <span v-if="errors.players" class="error">{{ errors.players }}</span>
      <li v-else v-for="player in players" :key="player.id">
        {{ player.name }}
        <button v-if="!player.requestedMatch" class="button-light" @click="requestGame(player)">Request Game</button>
        <button v-if="player.requestedMatch" class="button-light" @click="acceptMatching(player)">Accept match</button>
      </li>
    </ul>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import signalRHub from "@/services/signalRHub";

interface Player {
  id: number;
  name: string;
  requestedMatch: boolean;
}

export default defineComponent({
  async mounted() {
    signalRHub.invoke("GetPlayers");
  },
  data(): { players: Player[]; errors: { players: string } } {
    return {
      players: [
        { id: 1, name: "Player 1", requestedMatch: false },
        { id: 2, name: "Player 2", requestedMatch: false },
        { id: 3, name: "Player 3", requestedMatch: false },
      ],
      errors: { players: "" },
    };
  },
  methods: {
    async requestGame(player: Player) {
      console.log("Requesting game with player " + player.id);
      player.requestedMatch = true;
    },
    async acceptMatching(player: Player) {
      console.log("Accepted matching with player " + player.id);
      player.requestedMatch = false;
    },
  },
});
</script>

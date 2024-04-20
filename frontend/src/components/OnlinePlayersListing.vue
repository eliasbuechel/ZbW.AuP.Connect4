<template>
  <div class="listing-container">
    <h2>Online players</h2>
    <ul>
      <span v-if="errosr.players" class="error">{{ errors.players }}</span>
      <li v-else v-for="player in players" :key="player.id">
        {{ player.name }}
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

<script>
import signalRHub from "@/services/signalRHub";

export default {
  async mounted() {
    signalRHub.invoke("GetPlayers");

    // try {
    //   await this.$axios.get("http://localhost:5000/Player/get-online-players", {
    //     params: {
    //       email: "test",
    //     },
    //   });
    // } catch (error) {
    //   this.errors.players = "Not able to load players.";
    // }
  },
  data() {
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
    async requestGame(player) {
      console.log("Requesting game with player " + player.id);
      player.requestedMatch = true;
    },
    async acceptMatching(player) {
      console.log("Accepted matching with player " + player.id);
      player.requestedMatch = false;
    },
  },
};
</script>

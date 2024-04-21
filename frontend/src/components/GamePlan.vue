<template>
  <div class="listing-container">
    <h2>Game plan</h2>
    <ul>
      <li v-for="player in players" :key="player.id">
        <span>{{ player.email }} vs. {{ player.matching }}</span>
      </li>
    </ul>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";

interface GamePlanPlayer {
  id: number;
  email: string;
  matching: string;
}

export default defineComponent({
  name: "GamePlan",
  components: {},
  data(): { players: GamePlanPlayer[] } {
    return {
      players: [
        { id: 1, email: "Player 1", matching: "Player 2" },
        { id: 2, email: "Player 2", matching: "Player 5" },
        { id: 3, email: "Player 4", matching: "Player 2" },
      ],
    };
  },
  methods: {
    requestMatch(player: GamePlanPlayer) {
      // Simulate sending a match request to the selected player
      alert(`Match request sent to ${player.email}`);
    },
  },
});
</script>

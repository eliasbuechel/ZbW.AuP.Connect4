<template>
  <div class="listing-container">
    <h2>Game plan</h2>
    <ul>
      <li v-for="player in players" :key="player.id">
        <span>{{ player.name }} vs. {{ player.matching }}</span>
      </li>
    </ul>
  </div>
</template>

<script>
export default {
  data() {
    return {
      players: [
        { id: 1, name: "Player 1", matching: "Player 2" },
        { id: 2, name: "Player 2", matching: "Player 5" },
        { id: 3, name: "Player 4", matching: "Player 2" },
      ],
    };
  },
  methods: {
    requestMatch(player) {
      // Simulate sending a match request to the selected player
      alert(`Match request sent to ${player.name}`);
    },
  },
};
</script>

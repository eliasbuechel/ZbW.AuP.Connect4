<template>
  <div class="home">Die Hauptseite von R4D4</div>
  <OnlinePlayersListing />
  <GamePlan />
</template>

<script>
import OnlinePlayersListing from "@/components/OnlinePlayersListing.vue";
import GamePlan from "../components/GamePlan.vue";
// import signalRHub from "../services/signalRHub.js";

export default {
  components: {
    OnlinePlayersListing,
    GamePlan,
  },
};
</script>

<style></style>

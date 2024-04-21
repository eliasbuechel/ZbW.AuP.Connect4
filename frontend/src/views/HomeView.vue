<template>
  <div class="home">Die Hauptseite von R4D4</div>
  <OnlinePlayersListing />
  <GamePlan />
</template>

<script lang="ts">
import { defineComponent } from "vue";
import OnlinePlayersListing from "@/components/OnlinePlayersListing.vue";
import GamePlan from "../components/GamePlan.vue";
// import signalRHub from "../services/signalRHub.js";

export default defineComponent({
  name: "HomeView",
  components: {
    OnlinePlayersListing,
    GamePlan,
  },
});
</script>

<template>
  <img id="logo-home" src="@/assets/images/Logo.png" alt="r4d4-logo" />
  <div class="home">Die Hauptseite von R4D4</div>
  <UserInfo />
  <OnlinePlayersListing />
  <GamePlan />
</template>

<script lang="ts">
import { defineComponent } from "vue";
import OnlinePlayersListing from "@/components/OnlinePlayersListing.vue";
import GamePlan from "@/components/GamePlan.vue";
import signalRHub from "@/services/signalRHub";
import UserInfo from "@/components/UserInfo.vue";

export default defineComponent({
  mounted(): void {
    signalRHub.start();
  },
  unmounted() {
    signalRHub.stop();
  },
  components: {
    UserInfo,
    OnlinePlayersListing,
    GamePlan,
  },
});
</script>

<style></style>

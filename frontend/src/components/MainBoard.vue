<template>
  <div class="grid-container">
    <div class="grid-item-page-info page-info">
      <img id="logo-home" src="@/assets/images/Logo.png" alt="r4d4-logo" class="r4d4-logo" />
      <h1 class="home">R4D4 - Connect4</h1>
    </div>
    <UserInfo :identity="identity" class="grid-item-user-info" />
    <SinglePlayerModeSelection class="grid-item-signle-player-mode-selection" />
    <OnlinePlayersListing :onlinePlayers="onlinePlayers" :identity="identity" class="grid-item-online-player-listing" />
    <GamePlan class="grid-item-game-plan" :gamePlan="gamePlan" />
  </div>
</template>

<script lang="ts">
import { PropType, defineComponent } from "vue";
import OnlinePlayersListing from "@/components/OnlinePlayersListing.vue";
import GamePlan from "@/components/GamePlan.vue";
import UserInfo from "@/components/UserInfo.vue";
import { Match } from "@/types/Match";
import { OnlinePlayer } from "@/types/OnlinePlayer";
import { PlayerIdentity } from "@/types/PlayerIdentity";
import SinglePlayerModeSelection from "./SinglePlayerModeSelection.vue";

interface MainBoardState {
  isSubscribed: boolean;
}

export default defineComponent({
  name: "MainBoard",

  props: {
    identity: {
      required: true,
      type: Object as PropType<PlayerIdentity>,
    },
    onlinePlayers: {
      required: true,
      type: Array as PropType<OnlinePlayer[]>,
    },
    gamePlan: {
      required: true,
      type: Array as PropType<Match[]>,
    },
  },
  data(): MainBoardState {
    return {
      isSubscribed: false,
    };
  },
  components: {
    UserInfo,
    SinglePlayerModeSelection,
    OnlinePlayersListing,
    GamePlan,
  },
});
</script>

<style>
.grid-item-page-info {
  grid-column: 4 / span 6;
  grid-row: 1 / span 3;
}
.grid-item-user-info {
  grid-column: 10 / span 3;
  grid-row: 1 / span 3;
}
.grid-item-signle-player-mode-selection {
  grid-column: 4 / span 6;
  grid-row: 4 / span 3;
}

.grid-item-online-player-listing {
  grid-column: 1 / span 6;
  grid-row: 7 / span 6;
}
.grid-item-game-plan {
  grid-column: 7 / span 6;
  grid-row: 7 / span 6;
}
</style>

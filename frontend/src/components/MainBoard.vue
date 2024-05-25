<template>
  <div class="grid-container">
    <img id="logo-home" src="@/assets/images/Logo.png" alt="r4d4-logo" class="r4d4-logo grid-item-page-info" />
    <UserInfo :identity="identity" class="grid-item-user-info" />
    <BestList :bestlist="bestlist" @show-replay="showReplay" class="grid-item-best-list" />
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
import { GameResult } from "@/types/GameResult";
import BestList from "@/components/BestList.vue";

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
    bestlist: {
      required: true,
      type: Array as PropType<GameResult[]>,
    },
  },
  data(): MainBoardState {
    return {
      isSubscribed: false,
    };
  },
  components: {
    UserInfo,
    BestList,
    SinglePlayerModeSelection,
    OnlinePlayersListing,
    GamePlan,
  },
  methods: {
    showReplay(gameResult: GameResult): void {
      this.$emit("show-replay", gameResult);
    },
  },
});
</script>

<style>
.grid-item-page-info {
  grid-column: 1 / span 3;
  grid-row: 1 / span 2;
}
.grid-item-user-info {
  grid-column: 10 / span 3;
  grid-row: 1 / span 2;
}
.grid-item-signle-player-mode-selection {
  grid-column: 7 / span 6;
  grid-row: 3 / span 5;
}
.grid-item-best-list {
  grid-column: 1 / span 6;
  grid-row: 8 / span 5;
}
.grid-item-online-player-listing {
  grid-column: 1 / span 6;
  grid-row: 3 / span 5;
}
.grid-item-game-plan {
  grid-column: 7 / span 6;
  grid-row: 8 / span 5;
}
</style>
./BestList.vue@/types/GameResultMatch

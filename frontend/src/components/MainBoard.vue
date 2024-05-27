<template>
  <div class="main-board-container">
    <div class="header-container">
      <img id="logo-home" src="@/assets/images/Logo.png" alt="r4d4-logo" class="r4d4-logo" />
      <UserInfo :identity="identity" />
    </div>
    <SinglePlayerModeSelection class="content-card" />
    <OnlinePlayersListing :onlinePlayers="onlinePlayers" :identity="identity" class="content-card" />
    <GamePlan class="content-card" :gamePlan="gamePlan" />
    <BestList :bestlist="bestlist" @show-replay="showReplay" class="content-card" />
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

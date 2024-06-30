<template>
  <div class="header-container">
    <img id="logo-home" src="@/assets/images/Logo.png" alt="r4d4-logo" class="r4d4-logo" />
    <UserInfo :identity="identity" />
  </div>
  <div class="main-board-container">
    <SinglePlayerModeSelection class="content-card" :hasPendingRequest="hasPendingRequest" />
    <OnlinePlayersListing
      :onlinePlayers="connectedPlayers.webPlayers"
      :identity="identity"
      :hasPendingRequest="hasPendingRequest"
      class="content-card"
    />
    <OpponentRoboterPlayerListing
      :connectedOpponentRoboterPlayers="connectedPlayers.opponentRoboterPlayers"
      :identity="identity"
      class="content-card"
    />
    <GamePlan
      class="content-card"
      :gamePlan="gamePlan"
      :identity="identity"
      :isVisualizingOnRoboter="isVisualizingOnRoboter"
      @visualizing-on-roboter-changed="visualizingOnRoboterChanged"
    />
    <BestList :bestlist="bestlist" @show-replay="showReplay" class="content-card" />
  </div>
</template>

<script lang="ts">
import { PropType, defineComponent } from "vue";
import OnlinePlayersListingVue from "@/components/OnlinePlayersListingVue.vue";
import GamePlanVue from "@/components/GamePlanVue.vue";
import UserInfoVue from "@/components/UserInfoVue.vue";
import { Match } from "@/types/Match";
import { PlayerIdentity } from "@/types/PlayerIdentity";
import SinglePlayerModeSelectionVue from "./SinglePlayerModeSelectionVue.vue";
import { GameResult } from "@/types/GameResult";
import BestListVue from "@/components/BestListVue.vue";
import OpponentRoboterPlayerListingVue from "./OpponentRoboterPlayerListingVue.vue";
import { ConnectedPlayers } from "@/types/ConnectedPlayers";

interface MainBoardState {
  isSubscribed: boolean;
}

export default defineComponent({
  name: "MainBoardVue",
  props: {
    identity: {
      required: true,
      type: Object as PropType<PlayerIdentity>,
    },
    connectedPlayers: {
      required: true,
      type: Object as PropType<ConnectedPlayers>,
    },
    gamePlan: {
      required: true,
      type: Array as PropType<Match[]>,
    },
    bestlist: {
      required: true,
      type: Array as PropType<GameResult[]>,
    },
    isVisualizingOnRoboter: {
      required: true,
      type: Boolean,
    },
  },
  emits: ["show-replay", "visualizing-on-roboter-changed"],
  data(): MainBoardState {
    return {
      isSubscribed: false,
    };
  },
  components: {
    UserInfo: UserInfoVue,
    BestList: BestListVue,
    SinglePlayerModeSelection: SinglePlayerModeSelectionVue,
    OnlinePlayersListing: OnlinePlayersListingVue,
    OpponentRoboterPlayerListing: OpponentRoboterPlayerListingVue,
    GamePlan: GamePlanVue,
  },
  methods: {
    showReplay(gameResult: GameResult): void {
      this.$emit("show-replay", gameResult);
    },
    visualizingOnRoboterChanged(state: boolean): void {
      this.$emit("visualizing-on-roboter-changed", state);
    },
  },
  computed: {
    hasPendingRequest(): boolean {
      let doesHavePendingRequest: boolean = false;
      this.connectedPlayers.webPlayers.forEach((p) => {
        if (p.youRequestedMatch) doesHavePendingRequest = true;
      });
      return doesHavePendingRequest;
    },
  },
});
</script>

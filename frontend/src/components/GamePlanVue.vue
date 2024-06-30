<template>
  <div class="container">
    <div class="listing-container">
      <h2>Game plan</h2>
      <button
        v-if="gamePlan.length > 0 && !isGameParticipant(0)"
        class="button-light watch-button"
        @click="watchGame()"
      >
        Watch
      </button>
      <ToggleButton
        label="show on robot"
        :state="isVisualizingOnRoboter"
        @toggle-state="visualizingOnRoboterChanged"
        class="visualize-on-roboter-toggle-button"
      />
      <span v-if="gamePlan.length <= 0">No game planed.</span>
      <ul v-else>
        <li v-for="(player, idx) in gamePlan" :key="player.id" class="match">
          <div class="player player1">{{ player.player1.username }}</div>
          <div v-if="idx == 0" class="battle-icon">&#9876;</div>
          <div v-else class="handshake-icon">&#129309;</div>
          <div class="player player2">{{ player.player2.username }}</div>
        </li>
      </ul>
    </div>
  </div>
</template>

<script lang="ts">
import signalRHub from "@/services/signalRHub";
import { Match } from "@/types/Match";
import { PlayerIdentity } from "@/types/PlayerIdentity";
import { defineComponent, PropType } from "vue";
import ToggleButton from "./ToggleButton.vue";

interface GamePlanState {
  isSubscribed: boolean;
}

export default defineComponent({
  name: "GamePlanVue",
  props: {
    gamePlan: {
      required: true,
      type: Array as PropType<Match[]>,
    },
    identity: {
      required: true,
      type: Object as PropType<PlayerIdentity>,
    },
    isVisualizingOnRoboter: {
      required: true,
      type: Boolean,
    },
  },
  data(): GamePlanState {
    return {
      isSubscribed: false,
    };
  },
  components: {
    ToggleButton,
  },
  methods: {
    watchGame(): void {
      signalRHub.invoke("WatchGame");
    },
    isGameParticipant(idx: number): boolean {
      if (this.identity == null) return false;
      return this.gamePlan[idx].player1.id === this.identity.id || this.gamePlan[idx].player2.id === this.identity.id;
    },
    visualizingOnRoboterChanged(state: boolean): void {
      this.$emit("visualizing-on-roboter-changed", state);
    },
  },
});
</script>

<style scoped>
.match {
  display: flex;
  color: whitesmoke;
  text-align: center;
  justify-content: center;
  align-items: center;
}

.battle-icon {
  font-size: xx-large;
  color: brown;
  width: 50px;
  text-align: center;
}
.handshake-icon {
  font-size: x-large;
  width: 50px;
  text-align: center;
}

.match > .player {
  width: 240px;
  overflow: hidden;
  text-wrap: nowrap;
}

.match > .player1 {
  text-align: end;
}
.match > .player2 {
  text-align: start;
}

.listing-container {
  position: relative;
}

.watch-button {
  position: absolute;
  width: fit-content;
  top: 1.5rem;
  right: 1.5rem;
}

.visualize-on-roboter-toggle-button {
  position: absolute;
  top: 1.5rem;
  left: 1.5rem;
}
</style>
@/types/DataTransferObjects@/types/GameResultMatch

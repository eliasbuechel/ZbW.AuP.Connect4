<template>
  <div class="container">
    <div class="listing-container">
      <h2>Game plan</h2>
      <ul>
        <li v-for="(player, idx) in gamePlan" :key="player.id" class="match">
          <div>
            <div class="player1">{{ player.player1.username }}</div>
            <div v-if="idx == 0" class="battle-icon">&#9876;</div>
            <div v-else class="handshake-icon">&#129309;</div>
            <div class="player2">{{ player.player2.username }}</div>
          </div>
          <button v-if="idx === 0 && !isGameParticipant(idx)" class="button-light" @click="watchGame()">Watch</button>
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

interface GamePlanState {
  isSubscribed: boolean;
}

export default defineComponent({
  name: "GamePlan",
  props: {
    gamePlan: {
      required: true,
      type: Array as PropType<Match[]>,
    },
    identity: {
      required: true,
      type: Object as PropType<PlayerIdentity>,
    },
  },
  data(): GamePlanState {
    return {
      isSubscribed: false,
    };
  },
  methods: {
    watchGame(): void {
      signalRHub.invoke("WatchGame");
    },
    isGameParticipant(idx: number): boolean {
      if (this.identity == null) return false;
      console.log(this.identity.id, this.gamePlan[idx].player1.id, this.gamePlan[idx].player2.id);
      return this.gamePlan[idx].player1.id === this.identity.id || this.gamePlan[idx].player2.id === this.identity.id;
    },
  },
});
</script>

<style scoped>
.match {
  display: flex;
  color: whitesmoke;
  align-items: center;
  width: 100%;
}

.match > div {
  display: flex;
  align-items: center;
  justify-content: center;
  width: calc(100% - 80px);
}

.match > div > .battle-icon {
  font-size: xx-large;
  color: brown;
  width: 50px;
  text-align: center;
}
.match > div > .handshake-icon {
  font-size: x-large;
  width: 50px;
  text-align: center;
}

.match > div > .player1 {
  text-align: end;
  width: 50%;
}
.match > div > .player2 {
  width: 50%;
}
.match > button {
  margin-left: 0.3rem;
}
</style>
@/types/DataTransferObjects@/types/GameResultMatch

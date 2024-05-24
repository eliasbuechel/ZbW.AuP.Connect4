<template>
  <div class="container">
    <div class="listing-container">
      <h2>Game plan</h2>
      <ul>
        <li v-for="(player, idx) in gamePlan" :key="player.id" class="match">
          <div class="player1">{{ player.player1.username }}</div>
          <div v-if="idx == 0" class="battle-icon">&#9876;</div>
          <div v-else class="handshake-icon">&#129309;</div>
          <div class="player2">{{ player.player2.username }}</div>
        </li>
      </ul>
    </div>
  </div>
</template>

<script lang="ts">
import { Match } from "@/types/Match";
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
  },
  data(): GamePlanState {
    return {
      isSubscribed: false,
    };
  },
  methods: {},
});
</script>

<style scoped>
.match {
  display: flex;
  color: whitesmoke;
  align-items: center;
  width: 400px;
}

.match > .battle-icon {
  font-size: xx-large;
  color: brown;
  width: 50px;
  text-align: center;
}
.match > .handshake-icon {
  font-size: x-large;
  width: 50px;
  text-align: center;
}

.match > .player1 {
  text-align: end;
  width: 175px;
}
.match > .player2 {
  width: 175px;
}
</style>
@/types/DataTransferObjects@/types/GameResultMatch

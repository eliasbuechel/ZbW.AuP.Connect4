<template>
  <div class="listing-container">
    <h2>Game plan</h2>
    <ul>
      <li v-for="(player, idx) in gamePlan" :key="player.id" class="match">
        <span class="player1">{{ player.player1.username }}</span>
        <span v-if="idx == 0" class="battle-icon">&#9876;</span>
        <span v-else class="battle-icon">&#x1f91d;</span>
        <span class="player2">{{ player.player2.username }}</span>
      </li>
    </ul>
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
  justify-content: stretch;
}
.match > span {
  display: block;
}

.match > .battle-icon {
  font-size: xx-large;
  color: brown;
  flex-grow: 1;
}
match > .player1 {
  text-align: end;
  width: 200px;
}
match > .player2 {
}
</style>
@/types/DataTransferObjects

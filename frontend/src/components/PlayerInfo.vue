<template>
  <div :class="['player-info', playerClass]">
    <label>{{ playerName }}</label>

    <label class="move-label" v-if="isPlayerActive && currentMoveDuration"
      >Move: {{ currentMoveDuration }}</label
    >
    <label class="move-label" v-if="isPlayerActive && totalPlayTimeWithCurrentMove"
      >Total Time: {{ totalPlayTimeWithCurrentMove }}</label
    >

    <div class="playing-state">{{ gameState }}</div>
    <button v-if="isPlayerIdentity" class="button-light" @click="$emit('quitGame')">
      Quit game
    </button>
  </div>
</template>

<script>
  export default {
    props: {
      player: {
        type: Object,
        required: true,
      },
      identity: {
        type: Object,
        required: true,
      },
      gameState: {
        type: String,
        default: '',
      },
      currentMoveDuration: {
        type: Number,
        default: 0,
      },
      totalPlayTimeWithCurrentMove: {
        type: Number,
        default: 0,
      },
      playerClass: {
        type: String,
        default: '',
      },
    },
    methods: {},
    computed: {
      isPlayerActive() {
        return this.player.id === this.identity.id;
      },
      isPlayerIdentity() {
        return this.player.id === this.identity.id;
      },
      playerName() {
        if (this.player != null) {
          if (this.player.id == this.identity.id) return 'you';
          return this.player.username;
        }
        return '';
      },
    },
  };
</script>

<style scoped>
  .move-label {
    background-color: transparent;
  }
</style>

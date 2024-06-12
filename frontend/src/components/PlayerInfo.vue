<template>
  <div class="player-info">
    <label>{{ playerName }}</label>
    <div class="playing-state">{{ gameState }}</div>
    <button v-if="isPlayerInGame" class="button-light" @click="quitGame">Quit game</button>
  </div>
</template>

<script lang="ts">
  import { PropType, defineComponent } from "vue";
  import { InGamePlayer } from "@/types/InGamePlayer";
  import { PlayerIdentity } from "@/types/PlayerIdentity";
  import { Game } from "@/types/Game";

  export default defineComponent({
    props: {
      game: {
        required: true,
        type: Object as PropType<Game | undefined>,
        default: undefined,
      },
      player: {
        required: true,
        type: Object as PropType<InGamePlayer>,
      },
      identity: {
        required: true,
        type: Object as PropType<PlayerIdentity>,
      },
    },
    methods: {
      quitGame(): void {
        this.$emit("quit-game");
      },
    },
    computed: {
      playerName(): string {
        if (this.player != null) {
          if (this.player.id == this.identity.id) return "you";
          return this.player.username;
        }
        return "";
      },
      gameState(): string {
        if (this.game == null) return "";
        if (this.player == null) return "";
        if (!this.player.hasConfirmedGameStart)
          return this.player.id === this.identity.id
            ? "confirm to start the game"
            : "confirming game start ...";
        if (this.game.activePlayerId === this.player.id) {
          if (this.player.id == this.identity.id) return "your turn!";
          return "playing...";
        }
        return "";
      },
      isPlayerInGame(): boolean {
        if (this.game == null) return false;
        return (
          this.identity.id === this.game.match.player1.id || this.identity.id === this.game.match.player2.id
        );
      },
    },
  });
</script>

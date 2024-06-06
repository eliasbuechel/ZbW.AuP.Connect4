<template>
  <div class="grid-container">
    <div class="grid-item-page-info-container">
      <h2>Connect Four</h2>
      <span v-if="isGameStarted">Total game time: {{ formattedTime }}</span>
    </div>
    <PlayerInfo
      v-if="isGameStarted"
      class="grid-item-player1 player-info player-info-left"
      :player="inGamePlayerLeft"
      :identity="identity"
      :gameState="gameStatePlayerLeft"
      :currentMoveDuration="currentMoveDuration"
      :totalPlayTimeWithCurrentMove="totalPlayTimeWithCurrentMove"
      @quitGame="quitGame"
    />
    <PlayerInfo
      v-if="isGameStarted"
      class="grid-item-player2 player-info player-info-right"
      :player="inGamePlayerRight"
      :identity="identity"
      :gameState="gameStatePlayerRight"
      @quitGame="quitGame"
    />
    <button
      v-if="isGameReadyToStart"
      class="button-light grid-item-connect4-board confirm-game-start-button"
      @click="confirmGameStart"
    >
      Confirm game start
    </button>
    <Connect4Board
      v-if="game != null && game.startConfirmed"
      :identity="identity"
      :connect4Board="game.connect4Board"
      :playerLeft="inGamePlayerLeft!"
      :playerRight="inGamePlayerRight!"
      :activePlayerId="game.activePlayerId"
      @place-stone="reemitPlaceStone"
      @quit-game="reemitQuitGame"
      class="grid-item-connect4-board"
    />
  </div>
</template>

<script lang="ts">
  import { PropType, defineComponent } from 'vue';
  import { Game } from '@/types/Game';
  import { PlayerIdentity } from '@/types/PlayerIdentity';
  import Connect4Board from './Connect4Board.vue';
  import { InGamePlayer } from '@/types/InGamePlayer';
  import PlayerInfo from './PlayerInfo.vue';

  export default defineComponent({
    props: {
      game: {
        required: true,
        type: Object as PropType<Game | undefined>,
        default: undefined,
      },
      identity: {
        required: true,
        type: Object as PropType<PlayerIdentity>,
      },
    },
    data(): {
      currentMoveDuration: number;
      timerId?: number;
      minutes: number;
      secondes: number;
    } {
      return {
        currentMoveDuration: 0,
        timerId: undefined,
        minutes: 0,
        secondes: 0,
      };
    },
    components: {
      Connect4Board,
      PlayerInfo,
    },
    mounted() {
      this.calculateCurrentMoveDuration();
    },
    beforeUnmount() {
      clearTimeout(this.timerId);
    },
    methods: {
      reemitPlaceStone(column: number): void {
        if (this.game == null) return;
        if (this.game.activePlayerId !== this.identity.id) return;
        this.$emit('place-stone', column, this.currentMoveDuration);
      },
      confirmGameStart(): void {
        this.calculateTime();
        this.$emit('confirm-game-start');
      },
      reemitQuitGame(): void {
        if (this.game === undefined) return;
        // if (this.gameResult !== undefined) return;
        this.$emit('quit-game');
      },
      quitGame(): void {
        this.$emit('quit-game');
      },
      milisecondsToNow(): number {
        return Date.now() + 7200000;
      },
      calculateCurrentMoveDuration(): void {
        if (this.game == null) return;
        this.currentMoveDuration = parseFloat(
          ((this.milisecondsToNow() - this.game.moveStartTime) / 1000).toFixed(1)
        );
        this.timerId = setTimeout(this.calculateCurrentMoveDuration, 100);
      },
      calculateTime(): void {
        this.timerId = setInterval(() => {
          this.secondes++;
          if (this.secondes === 60) {
            this.secondes = 0;
            this.minutes++;
          }
        }, 1000);
      },
    },
    computed: {
      isGameStarted() {
        return this.game != null && this.game.startConfirmed;
      },
      isGameReadyToStart() {
        return this.game != null && !this.inGamePlayerLeft?.hasConfirmedGameStart;
      },
      inGamePlayerLeft(): InGamePlayer | undefined {
        if (this.game != null) {
          return this.game.match.player1.id == this.identity.id
            ? this.game.match.player1
            : this.game.match.player2;
        }

        return undefined;
      },
      inGamePlayerRight(): InGamePlayer | undefined {
        if (this.game != null) {
          return this.game.match.player1.id == this.identity.id
            ? this.game.match.player2
            : this.game.match.player1;
        }

        return undefined;
      },
      totalPlayTimeWithCurrentMove(): number {
        if (this.inGamePlayerLeft == null) return 0;
        return parseFloat(
          (this.inGamePlayerLeft.totalPlayTime + this.currentMoveDuration).toFixed(1)
        );
      },
      // gameResultPlayerLeft(): PlayerIdentity | undefined {
      //   if (this.gameResult != null)
      //     return this.gameResult.match.player1.id == this.identity.id
      //       ? this.gameResult.match.player1
      //       : this.gameResult.match.player2;

      //   return undefined;
      // },
      // gameResultPlayerRight(): PlayerIdentity | undefined {
      //   if (this.gameResult != null)
      //     return this.gameResult.match.player1.id == this.identity.id
      //       ? this.gameResult.match.player2
      //       : this.gameResult.match.player1;

      //   return undefined;
      // },
      gameStatePlayerLeft(): string {
        if (this.game == null) return '';
        if (this.inGamePlayerLeft == null) return '';
        if (!this.inGamePlayerLeft.hasConfirmedGameStart)
          return this.inGamePlayerLeft.id === this.identity.id
            ? 'confirm to start the game'
            : 'confirming game start ...';
        if (!this.game.startConfirmed) return '';
        if (this.game.activePlayerId === this.inGamePlayerLeft.id) {
          if (this.inGamePlayerLeft.id == this.identity.id) return 'your turn!';
          return 'playing...';
        }
        return '';
      },
      gameStatePlayerRight(): string {
        if (this.game == null) return '';
        if (this.inGamePlayerRight == null) return '';
        if (!this.inGamePlayerRight.hasConfirmedGameStart)
          return this.inGamePlayerRight.id === this.identity.id
            ? 'confirm to start the game'
            : 'confirming game start ...';
        if (!this.game.startConfirmed) return '';
        if (this.game.activePlayerId === this.inGamePlayerRight.id) {
          if (this.inGamePlayerRight.id == this.identity.id) return 'your turn!';
          return 'playing...';
        }
        return '';
      },
      activePlayer(): InGamePlayer | undefined {
        if (this.game == null) return undefined;
        if (this.game.activePlayerId === this.game.match.player1.id) return this.game.match.player1;
        return this.game.match.player2;
      },
      formattedTime(): string {
        let minutes = this.minutes.toString().padStart(2, '0');
        let seconds = this.secondes.toString().padStart(2, '0');
        return `${minutes}:${seconds}`;
      },
    },
  });
</script>

<style scoped>
  .grid-item-page-info-container {
    grid-column: 4 / span 6;
    grid-row: 1 / span 2;
  }

  .grid-item-player1 {
    grid-column: 1 / span 3;
    grid-row: 1 / span 4;
  }

  .grid-item-player2 {
    grid-column: 10 / span 3;
    grid-row: 1 / span 4;
  }

  .grid-item-connect4-board {
    grid-column: 1 / span 12;
    grid-row: 3 / span 10;
  }

  .grid-item-game-result {
    grid-column: 2 / span 10;
    grid-row: 3 / span 10;
  }

  .confirm-game-start-button {
    height: 3rem;
    padding: 1rem;
    width: fit-content;
    justify-self: center;
    align-self: center;
  }

  .move-label {
    background-color: unset;
    font-size: small;
  }
</style>

<template>
  <MainBoard
    v-if="!isInGame && identity != null"
    :identity="identity"
    :onlinePlayers="onlinePlayers"
    :gamePlan="gamePlan"
  />
  <Connect4Game
    v-else-if="isInGame && identity != null && (game != null || gameResult != null)"
    :game="game"
    :gameResult="gameResult"
    :identity="identity"
    @place-stone="placeStone"
    @quit-game="quitGame"
    @leave-game-result-view="leaveGameResultView"
  />
</template>

<script lang="ts">
import { defineComponent } from "vue";
import signalRHub from "@/services/signalRHub";
import MainBoard from "@/components/MainBoard.vue";
import Connect4Game from "@/components/Connect4Game.vue";
import eventBus from "@/services/eventBus";
import { PlayerIdentity } from "@/types/PlayerIdentity";
import { Game } from "@/types/Game";
import { Match } from "@/types/Match";
import { OnlinePlayer } from "@/types/OnlinePlayer";
import { GameResult } from "@/types/GameResult";
import { Field } from "@/types/Field";

interface HomeState {
  identity?: PlayerIdentity;
  onlinePlayers: OnlinePlayer[];
  gamePlan: Match[];
  game?: Game;
  gameResult?: GameResult;
  isSubscribed: boolean;
}

export default defineComponent({
  name: "HomeView",
  data(): HomeState {
    return {
      identity: undefined,
      onlinePlayers: new Array<OnlinePlayer>(),
      gamePlan: new Array<Match>(),
      game: undefined,
      gameResult: undefined,
      isSubscribed: false,
    };
  },
  components: {
    MainBoard,
    Connect4Game,
  },
  mounted(): void {
    if (signalRHub.isConnected()) {
      this.subscribe();
    } else {
      signalRHub.start();
    }

    eventBus.on("signalr-connected", this.onSignalRConnected);
    eventBus.on("signalr-disconnected", this.onSignalRDisconnected);
  },
  unmounted() {
    eventBus.off("signalr-connected", this.onSignalRConnected);
    eventBus.off("signalr-disconnected", this.onSignalRDisconnected);

    signalRHub.stop();
    this.unsubscribe();
  },
  methods: {
    subscribe(): void {
      if (this.isSubscribed) return;
      signalRHub.on("game-started", this.onGameStarted);
      signalRHub.on("game-ended", this.onGameEnded);
      signalRHub.on("send-current-game", this.updateGame);
      signalRHub.on("send-user-data", this.updateUserIdentity);
      signalRHub.on("send-game-plan", this.onUpdateGamePlan);
      signalRHub.on("matched", this.onMatched);
      signalRHub.on("player-disconnected", this.onPlayerDisconnected);
      signalRHub.on("move-played", this.onMovePlayed);

      signalRHub.on("send-online-players", this.onUdateOnlinePlayers);
      signalRHub.on("player-connected", this.onPlayerConnected);
      signalRHub.on("player-requested-match", this.onPlayerRequestedMatch);
      signalRHub.on("you-requested-match", this.onYouRequestedMatch);
      signalRHub.on("player-rejected-match", this.onPlayerRejectedMatch);
      signalRHub.on("you-rejected-match", this.onYouRejectedMatch);
    },
    unsubscribe(): void {
      if (!this.isSubscribed) return;
      signalRHub.off("game-started", this.onGameStarted);
      signalRHub.off("game-ended", this.onGameEnded);
      signalRHub.off("send-current-game", this.updateGame);
      signalRHub.off("send-user-data", this.updateUserIdentity);
      signalRHub.off("send-game-plan", this.onUpdateGamePlan);
      signalRHub.off("matched", this.onMatched);
      signalRHub.off("player-disconnected", this.onPlayerDisconnected);

      signalRHub.off("send-online-players", this.onUdateOnlinePlayers);
      signalRHub.off("player-connected", this.onPlayerConnected);
      signalRHub.off("player-requested-match", this.onPlayerRequestedMatch);
      signalRHub.off("you-requested-match", this.onYouRequestedMatch);
      signalRHub.off("player-rejected-match", this.onPlayerRejectedMatch);
      signalRHub.off("you-rejected-match", this.onYouRejectedMatch);
    },
    leaveGameResultView(): void {
      this.gameResult = undefined;
    },
    placeStone(column: number): void {
      signalRHub.invoke("PlayMove", column);
    },
    quitGame(): void {
      signalRHub.invoke("QuitGame");
    },
    updateUserIdentity(identity: PlayerIdentity): void {
      this.identity = identity;
    },
    updateGame(game: Game | null): void {
      if (!game) return;
      this.game = game;
    },
    onGameStarted(game: Game): void {
      this.gameResult = undefined;
      this.game = game;
      console.log(game);
    },
    onYouQuitGame(): void {
      if (this.game === undefined) {
        this.game = undefined;
        this.popGamePlan();
        return;
      }
      this.gameResult = undefined;
    },
    onQuitGame(): void {
      if (this.game === undefined) {
        this.game = undefined;
        this.popGamePlan();
        return;
      }
    },
    popGamePlan(): void {
      this.gamePlan = this.gamePlan.filter((m, idx) => idx != 0);
    },
    onUpdateGamePlan(gamePlan: Match[]) {
      this.gamePlan = gamePlan;
      if (this.gamePlan.length > 0) signalRHub.invoke("GetCurrentGame");
    },
    onMatched(match: Match): void {
      this.gamePlan = new Array<Match>(...this.gamePlan, match);

      if (this.identity === undefined) return;
      this.onlinePlayers.forEach((p) => {
        if (
          (p.id === match.player1.id && this.identity?.id === match.player2.id) ||
          (p.id === match.player2.id && this.identity?.id === match.player1.id)
        ) {
          p.matched = true;
          p.requestedMatch = false;
          p.youRequestedMatch = false;
          return;
        }
      });
    },
    onGameEnded(gameResult: GameResult): void {
      if (this.isInGame != null) {
        this.gameResult = gameResult;
        this.game = undefined;
      }
      const player1: PlayerIdentity | undefined = this.gamePlan.at(0)?.player1;
      const player2: PlayerIdentity | undefined = this.gamePlan.at(0)?.player2;

      if (player1 != null && player2 != null && this.identity != null) {
        const opponent = player1.id === this.identity.id ? player2 : player1;

        this.onlinePlayers.forEach((p) => {
          if (p.id === opponent.id) p.matched = false;
        });
      }

      this.popGamePlan();
    },
    onPlayerDisconnected(playerId: string): void {
      this.onlinePlayers = this.onlinePlayers.filter((o) => o.id !== playerId);
      this.gamePlan = this.gamePlan.filter((m) => m.player1.id !== playerId && m.player2.id !== playerId);
    },
    onMovePlayed(playerId: string, field: Field): void {
      if (this.game == null) return;
      if (this.identity == null) return;

      this.game!.connect4Board[field.column][field.row] = playerId;
      this.switchActivePlayer();
    },
    switchActivePlayer(): void {
      this.game!.activePlayerId =
        this.game!.activePlayerId === this.game!.match.player1.id
          ? this.game!.match.player2.id
          : this.game!.match.player1.id;
    },
    onUdateOnlinePlayers(onlinePlayers: OnlinePlayer[]): void {
      this.onlinePlayers = onlinePlayers;
    },
    onPlayerConnected(onlinePlayer: OnlinePlayer): void {
      this.onlinePlayers = new Array<OnlinePlayer>(...this.onlinePlayers, onlinePlayer);
    },
    onPlayerRequestedMatch(playerId: string): void {
      this.onlinePlayers.forEach((p) => {
        if (p.id === playerId) {
          p.requestedMatch = true;
          return;
        }
      });
    },
    onYouRequestedMatch(playerId: string): void {
      this.onlinePlayers.forEach((p) => {
        if (p.id === playerId) {
          p.youRequestedMatch = true;
          return;
        }
      });
    },
    onPlayerRejectedMatch(playerId: string): void {
      this.onlinePlayers.forEach((p) => {
        if (p.id === playerId) {
          p.youRequestedMatch = false;
          return;
        }
      });
    },
    onYouRejectedMatch(playerId: string): void {
      this.onlinePlayers.forEach((p) => {
        if (p.id === playerId) {
          p.requestedMatch = false;
          return;
        }
      });
    },
    onSignalRConnected(): void {
      this.subscribe();
      signalRHub.invoke("GetGamePlan");
      signalRHub.invoke("HasGameStarted");
      signalRHub.invoke("GetUserData");
      signalRHub.invoke("GetOnlinePlayers");
    },
    onSignalRDisconnected(): void {
      this.unsubscribe();
    },
  },
  computed: {
    isInGame(): boolean {
      return (
        (this.game != null &&
          this.identity != null &&
          (this.game.match.player1.id === this.identity.id || this.game.match.player2.id === this.identity.id)) ||
        this.gameResult != null
      );
    },
  },
});
</script>

<style scoped></style>

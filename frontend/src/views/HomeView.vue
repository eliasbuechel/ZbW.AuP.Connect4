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
      signalRHub.on("PlayerConnected", this.onPlayerConnected);
      signalRHub.on("PlayerDisconnected", this.onPlayerDisconnected);
      signalRHub.on("PlayerRequestedMatch", this.onPlayerRequestedMatch);
      signalRHub.on("PlayerRejectedMatch", this.onPlayerRejectedMatch);
      signalRHub.on("Matched", this.onMatched);
      signalRHub.on("MatchingEnded", this.onMatchingEnded);
      signalRHub.on("MovePlayed", this.onMovePlayed);
      signalRHub.on("GameStarted", this.onGameStarted);
      signalRHub.on("GameEnded", this.onGameEnded);
      signalRHub.on("SendUserData", this.updateUserIdentity);
      signalRHub.on("SendOnlinePlayers", this.onUdateOnlinePlayers);
      signalRHub.on("SendGamePlan", this.onUpdateGamePlan);
      signalRHub.on("SendGame", this.updateGame);
      signalRHub.on("YouRequestedMatch", this.onYouRequestedMatch);
      signalRHub.on("YouRejectedMatch", this.onYouRejectedMatch);
    },
    unsubscribe(): void {
      if (!this.isSubscribed) return;
      signalRHub.off("PlayerConnected", this.onPlayerConnected);
      signalRHub.off("PlayerDisconnected", this.onPlayerDisconnected);
      signalRHub.off("PlayerRequestedMatch", this.onPlayerRequestedMatch);
      signalRHub.off("PlayerRejectedMatch", this.onPlayerRejectedMatch);
      signalRHub.off("Matched", this.onMatched);
      signalRHub.off("MatchingEnded", this.onMatchingEnded);
      signalRHub.off("MovePlayed", this.onMovePlayed);
      signalRHub.off("GameStarted", this.onGameStarted);
      signalRHub.off("GameEnded", this.onGameEnded);
      signalRHub.off("SendUserData", this.updateUserIdentity);
      signalRHub.off("SendOnlinePlayers", this.onUdateOnlinePlayers);
      signalRHub.off("SendGamePlan", this.onUpdateGamePlan);
      signalRHub.off("SendGame", this.updateGame);
      signalRHub.off("YouRequestedMatch", this.onYouRequestedMatch);
      signalRHub.off("YouRejectedMatch", this.onYouRejectedMatch);
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
        // this.popGamePlan();
        return;
      }
      this.gameResult = undefined;
    },
    onQuitGame(): void {
      if (this.game === undefined) {
        this.game = undefined;
        // this.popGamePlan();
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
    onMatchingEnded(matchId: string): void {
      const matches: Match[] = this.gamePlan.filter((m) => m.id === matchId);
      if (matches.length >= 1) {
        const match: Match = matches[0];

        if (this.identity == null) return;
        if (!(match.player1.id !== this.identity.id && match.player2.id !== this.identity.id)) {
          const opponent: PlayerIdentity = match.player1.id === this.identity.id ? match.player2 : match.player1;

          this.onlinePlayers.filter((p) => p.id === opponent.id).forEach((p) => (p.matched = false));
        }
      }

      this.gamePlan = this.gamePlan.filter((m) => m.id !== matchId);
    },
    onGameEnded(gameResult: GameResult): void {
      if (this.isInGame != null) {
        this.gameResult = gameResult;
        this.game = undefined;
      }
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
      signalRHub.invoke("GetUserData");
      signalRHub.invoke("GetOnlinePlayers");
      signalRHub.invoke("GetGamePlan");
      signalRHub.invoke("GetGame");
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

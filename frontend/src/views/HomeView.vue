<template>
  <div class="home-view-container">
    <LoadingScreen v-if="identity == null || bestlist == null" />
    <GameResultView
      v-else-if="gameResult != null"
      :gameResult="gameResult"
      :identity="identity"
      @leave-game-result-view="leaveGameResultView"
    />
    <Connect4Game
      v-else-if="game != null"
      :game="game"
      :identity="identity"
      @place-stone="placeStone"
      @quit-game="quitGame"
      @confirm-game-start="confirmGameStart"
      @stop-watching-game="stopWatchingGame"
    />
    <MainBoard
      v-else-if="bestlist != null"
      :identity="identity"
      :bestlist="bestlist"
      :connectedPlayers="connectedPlayers"
      :gamePlan="gamePlan"
      @show-replay="showReplay"
    />
  </div>
</template>

<script lang="ts">
  import { defineComponent } from "vue";
  import signalRHub from "@/services/signalRHub";
  import MainBoardVue from "@/components/MainBoardVue.vue";
  import GameVue from "@/components/GameVue.vue";
  import eventBus from "@/services/eventBus";
  import { PlayerIdentity } from "@/types/PlayerIdentity";
  import Game, { IGame } from "@/types/Game";
  import { Match } from "@/types/Match";
  import { OnlinePlayer } from "@/types/OnlinePlayer";
  import { GameResult } from "@/types/GameResult";
  import { Field } from "@/types/Field";
  import GameResultView from "@/components/GameResultView.vue";
  import { ConnectedPlayers } from "@/types/ConnectedPlayers";
  import LoadingScreenVue from "@/components/LoadingScreenVue.vue";

  interface HomeState {
    identity?: PlayerIdentity;
    connectedPlayers: ConnectedPlayers;
    bestlist?: GameResult[];
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
        connectedPlayers: {
          webPlayers: new Array<OnlinePlayer>(),
          opponentRoboterPlayers: new Array<OnlinePlayer>(),
        },
        bestlist: new Array<GameResult>(),
        gamePlan: new Array<Match>(),
        game: undefined,
        gameResult: undefined,
        isSubscribed: false,
      };
    },
    components: {
      LoadingScreen: LoadingScreenVue,
      MainBoard: MainBoardVue,
      Connect4Game: GameVue,
      GameResultView,
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

      this.unsubscribe();
      signalRHub.stop();
    },
    methods: {
      subscribe(): void {
        if (this.isSubscribed) return;
        signalRHub.on("PlayerConnected", this.onPlayerConnected);
        signalRHub.on("PlayerDisconnected", this.onPlayerDisconnected);
        signalRHub.on("OpponentRoboterPlayerConnected", this.onOpponentRoboterPlayerConnected);
        signalRHub.on("OpponentRoboterPlayerDisconnected", this.onOpponentRoboterPlayerDisconnected);
        signalRHub.on("PlayerRequestedMatch", this.onPlayerRequestedMatch);
        signalRHub.on("PlayerRejectedMatch", this.onPlayerRejectedMatch);
        signalRHub.on("Matched", this.onMatched);
        signalRHub.on("MatchingEnded", this.onMatchingEnded);
        signalRHub.on("MovePlayed", this.onMovePlayed);
        signalRHub.on("PlacingStone", this.onPlacingStone);
        signalRHub.on("GameStarted", this.onGameStarted);
        signalRHub.on("GameEnded", this.onGameEnded);
        signalRHub.on("SendUserData", this.updateUserIdentity);
        signalRHub.on("SendConnectedPlayers", this.onUdateConnectedPlayers);
        signalRHub.on("SendGamePlan", this.onUpdateGamePlan);
        signalRHub.on("SendGame", this.updateGame);
        signalRHub.on("YouRequestedMatch", this.onYouRequestedMatch);
        signalRHub.on("YouRejectedMatch", this.onYouRejectedMatch);
        signalRHub.on("ConfirmedGameStart", this.onConfirmedGameStart);
        signalRHub.on("SendHint", this.onSendHint);
        signalRHub.on("SendBestlist", this.onSendBestlist);
        signalRHub.on("YouStoppedWatchingGame", this.onYouStoppedWatchingGame);
        signalRHub.on("NotAbleToConnectToOpponentRoboterPlayer", this.onNotAbleToConnectToOpponentRoboterPlayer);
        signalRHub.on("RequestErrorOccured", this.onRequestErrorOccured);
        signalRHub.on("RedirectToLogin", this.onRedirectToLogin);
      },
      unsubscribe(): void {
        if (!this.isSubscribed) return;
        signalRHub.off("PlayerConnected", this.onPlayerConnected);
        signalRHub.off("PlayerDisconnected", this.onPlayerDisconnected);
        signalRHub.off("OpponentRoboterPlayerConnected", this.onOpponentRoboterPlayerConnected);
        signalRHub.off("OpponentRoboterPlayerDisconnected", this.onOpponentRoboterPlayerDisconnected);
        signalRHub.off("PlayerRequestedMatch", this.onPlayerRequestedMatch);
        signalRHub.off("PlayerRejectedMatch", this.onPlayerRejectedMatch);
        signalRHub.off("Matched", this.onMatched);
        signalRHub.off("MatchingEnded", this.onMatchingEnded);
        signalRHub.off("MovePlayed", this.onMovePlayed);
        signalRHub.on("PlacingStone", this.onPlacingStone);
        signalRHub.off("GameStarted", this.onGameStarted);
        signalRHub.off("GameEnded", this.onGameEnded);
        signalRHub.off("SendUserData", this.updateUserIdentity);
        signalRHub.off("SendConnectedPlayers", this.onUdateConnectedPlayers);
        signalRHub.off("SendGamePlan", this.onUpdateGamePlan);
        signalRHub.off("SendGame", this.updateGame);
        signalRHub.off("YouRequestedMatch", this.onYouRequestedMatch);
        signalRHub.off("YouRejectedMatch", this.onYouRejectedMatch);
        signalRHub.off("ConfirmedGameStart", this.onConfirmedGameStart);
        signalRHub.off("SendHint", this.onSendHint);
        signalRHub.off("SendBestlist", this.onSendBestlist);
        signalRHub.off("YouStoppedWatchingGame", this.onYouStoppedWatchingGame);
        signalRHub.off("NotAbleToConnectToOpponentRoboterPlayer", this.onNotAbleToConnectToOpponentRoboterPlayer);
        signalRHub.off("RequestError", this.onRequestErrorOccured);
        signalRHub.off("RedirectToLogin", this.onRedirectToLogin);
      },
      leaveGameResultView(): void {
        this.gameResult = undefined;
      },
      confirmGameStart(): void {
        signalRHub.invoke("ConfirmGameStart");
      },
      stopWatchingGame(): void {
        this.game = undefined;
      },
      placeStone(column: number, playerId: string): void {
        signalRHub.invoke("PlayMove", column);
        if (this.game == null) return;

        this.game.match.player1.currentHint = undefined;
        this.game.match.player2.currentHint = undefined;

        if (this.game.moveStartTime != null) {
          if (this.game.match.player1.id === playerId) {
            if (this.game.match.player1.totalPlayTime != null) {
              this.game.match.player1.totalPlayTime += Date.now() - this.game.moveStartTime;
            }
          }
          if (this.game.match.player2.id === playerId) {
            if (this.game.match.player2.totalPlayTime != null) {
              this.game.match.player2.totalPlayTime += Date.now() - this.game.moveStartTime;
            }
          }
        }

        this.game.moveStartTime = undefined;
      },
      quitGame(): void {
        signalRHub.invoke("QuitGame");
      },
      updateUserIdentity(identity: PlayerIdentity): void {
        this.identity = identity;
      },
      updateGame(game?: IGame): void {
        if (game == null) return;
        this.game = new Game(game);
      },
      onGameStarted(game: IGame): void {
        this.gameResult = undefined;
        this.game = new Game(game);
      },
      onYouQuitGame(): void {
        if (this.game === undefined) {
          this.game = undefined;
          return;
        }
        this.gameResult = undefined;
      },
      onQuitGame(): void {
        if (this.game === undefined) {
          this.game = undefined;
          return;
        }
      },
      popGamePlan(): void {
        this.gamePlan = this.gamePlan.filter((m, idx) => idx != 0);
      },
      onUpdateGamePlan(gamePlan: Match[]) {
        this.gamePlan = gamePlan;
        if (this.gamePlan.length > 0) signalRHub.invoke("GetGame");
      },
      showReplay(gameResult: GameResult): void {
        this.gameResult = gameResult;
      },
      onMatched(match: Match): void {
        this.gamePlan = new Array<Match>(...this.gamePlan, match);

        if (this.identity === undefined) return;

        this.connectedPlayers.webPlayers.forEach((p) => {
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

        this.connectedPlayers.opponentRoboterPlayers.forEach((p) => {
          if (p.id === match.player1.id || p.id === match.player2.id) {
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

            this.connectedPlayers.webPlayers
              .filter((p) => p.id === opponent.id)
              .forEach((p) => (p.matched = false));
          }
        }

        this.gamePlan = this.gamePlan.filter((m) => m.id !== matchId);
      },
      onGameEnded(gameResult: GameResult): void {
        if (this.identity == null) return;

        if (this.game != null) {
          this.gameResult = gameResult;
          this.game = undefined;
        }

        this.gamePlan = this.gamePlan.filter((x, i) => i != 0);

        this.connectedPlayers.webPlayers.forEach((p) => {
          if (gameResult.match.player1.id === this.identity!.id && p.id === gameResult.match.player2.id)
            p.matched = false;
          if (gameResult.match.player2.id === this.identity!.id && p.id === gameResult.match.player1.id)
            p.matched = false;
        });

        this.connectedPlayers.opponentRoboterPlayers.forEach((p) => {
          if (p.id === gameResult.match.player1.id || p.id === gameResult.match.player2.id) p.matched = false;
        });
      },
      onMovePlayed(playerId: string, field: Field): void {
        if (this.game == null) return;
        if (this.identity == null) return;

        this.game.placingField = undefined;
        this.game.lastPlacedStone = field;
        this.game.moveStartTime = Date.now();
        this.game.match.player1.currentHint = undefined;
        this.game.match.player2.currentHint = undefined;
        this.game.board[field.column][field.row] = playerId;
        this.switchActivePlayer();
      },
      onPlacingStone(playerId: string, field: Field): void {
        if (this.game == null) return;

        this.game.placingField = field;
        this.game.match.player1.currentHint = undefined;
        this.game.match.player2.currentHint = undefined;

        if (this.game.moveStartTime != null) {
          if (this.game.match.player1.id === playerId) {
            if (this.game.match.player1.totalPlayTime != null) {
              this.game.match.player1.totalPlayTime += Date.now() - this.game.moveStartTime;
            }
          }
          if (this.game.match.player2.id === playerId) {
            if (this.game.match.player2.totalPlayTime != null) {
              this.game.match.player2.totalPlayTime += Date.now() - this.game.moveStartTime;
            }
          }
        }

        this.game.moveStartTime = undefined;
      },
      switchActivePlayer(): void {
        this.game!.activePlayerId =
          this.game!.activePlayerId === this.game!.match.player1.id
            ? this.game!.match.player2.id
            : this.game!.match.player1.id;
      },
      onUdateConnectedPlayers(connectedPlayers: ConnectedPlayers): void {
        this.connectedPlayers = connectedPlayers;
      },
      onPlayerConnected(onlinePlayer: OnlinePlayer): void {
        this.connectedPlayers.webPlayers = new Array<OnlinePlayer>(
          ...this.connectedPlayers.webPlayers,
          onlinePlayer
        );
      },
      onPlayerDisconnected(playerId: string): void {
        this.connectedPlayers.webPlayers = this.connectedPlayers.webPlayers.filter((o) => o.id !== playerId);
        this.gamePlan = this.gamePlan.filter((m) => m.player1.id !== playerId && m.player2.id !== playerId);
      },
      onOpponentRoboterPlayerConnected(onlinePlayer: OnlinePlayer): void {
        this.connectedPlayers.opponentRoboterPlayers = new Array<OnlinePlayer>(
          ...this.connectedPlayers.opponentRoboterPlayers,
          onlinePlayer
        );
      },
      onOpponentRoboterPlayerDisconnected(playerId: string): void {
        this.connectedPlayers.opponentRoboterPlayers = this.connectedPlayers.opponentRoboterPlayers.filter(
          (o) => o.id !== playerId
        );
        this.gamePlan = this.gamePlan.filter((m) => m.player1.id !== playerId && m.player2.id !== playerId);
      },
      onPlayerRequestedMatch(playerId: string): void {
        this.connectedPlayers.webPlayers.forEach((p) => {
          if (p.id === playerId) {
            p.requestedMatch = true;
            return;
          }
        });
        this.connectedPlayers.opponentRoboterPlayers.forEach((p) => {
          if (p.id === playerId) {
            p.requestedMatch = true;
            return;
          }
        });
      },
      onYouRequestedMatch(playerId: string): void {
        this.connectedPlayers.webPlayers.forEach((p) => {
          if (p.id === playerId) {
            p.youRequestedMatch = true;
            return;
          }
        });
        this.connectedPlayers.opponentRoboterPlayers.forEach((p) => {
          if (p.id === playerId) {
            p.youRequestedMatch = true;
            return;
          }
        });
      },
      onPlayerRejectedMatch(playerId: string): void {
        this.connectedPlayers.webPlayers.forEach((p) => {
          if (p.id === playerId) {
            p.youRequestedMatch = false;
            return;
          }
        });
        this.connectedPlayers.opponentRoboterPlayers.forEach((p) => {
          if (p.id === playerId) {
            p.youRequestedMatch = false;
            return;
          }
        });
      },
      onYouRejectedMatch(playerId: string): void {
        this.connectedPlayers.webPlayers.forEach((p) => {
          if (p.id === playerId) {
            p.requestedMatch = false;
            return;
          }
        });
        this.connectedPlayers.opponentRoboterPlayers.forEach((p) => {
          if (p.id === playerId) {
            p.requestedMatch = false;
            return;
          }
        });
      },
      onConfirmedGameStart(playerId: string): void {
        if (this.game == null) return;
        if (this.game.match.player1.id === playerId) this.game.match.player1.hasConfirmedGameStart = true;
        else this.game.match.player2.hasConfirmedGameStart = true;

        if (this.game.match.player1.hasConfirmedGameStart && this.game.match.player2.hasConfirmedGameStart) {
          this.game.gameStartTime = Date.now();
          this.game.moveStartTime = Date.now();
          this.game.match.player1.totalPlayTime = 0;
          this.game.match.player2.totalPlayTime = 0;
        }
      },
      onSendHint(hint: number): void {
        if (this.identity == null) return;
        if (this.game == null) return;

        if (this.game.match.player1.id === this.identity.id) {
          this.game.match.player1.currentHint = hint;
          this.game.match.player1.hintsLeft--;
        }
        if (this.game.match.player2.id === this.identity.id) {
          this.game.match.player2.currentHint = hint;
          this.game.match.player2.hintsLeft--;
        }
      },
      onSendBestlist(bestlist: GameResult[]): void {
        this.bestlist = bestlist;
      },
      onYouStoppedWatchingGame(): void {
        this.game = undefined;
      },
      onNotAbleToConnectToOpponentRoboterPlayer(errorMessage: string): void {
        eventBus.emit("NotAbleToConnectToOpponentRoboterPlayer", errorMessage);
      },
      addToBestlist(gameResult: GameResult): void {
        if (this.bestlist == null) return;

        this.bestlist = new Array<GameResult>(gameResult, ...this.bestlist);
      },
      onRequestErrorOccured(): void {
        location.reload();
      },
      onRedirectToLogin(): void {
        this.$router.push({ name: "Login" });
      },
      onSignalRConnected(): void {
        this.subscribe();
        signalRHub.invoke("GetUserData");
        signalRHub.invoke("GetConnectedPlayers");
        signalRHub.invoke("GetGamePlan");
        signalRHub.invoke("GetGame");
        signalRHub.invoke("GetBestlist");
      },
      onSignalRDisconnected(): void {
        this.unsubscribe();
      },
    },
    computed: {
      isInGame(): boolean {
        return (this.game != null && this.identity != null) || this.gameResult != null;
      },
    },
  });
</script>

<style scoped></style>
@/types/GameResultMatch

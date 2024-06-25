import Entity, { IEntity } from "./Entity";
import { Field } from "./Field";
import { InGamePlayer } from "./InGamePlayer";
import { Match } from "./Match";
import { PlayerIdentity } from "./PlayerIdentity";

export interface IGame extends IEntity {
  match: Match;
  activePlayerId: string;
  board: string[][];
  moveStartTime?: number;
  gameStartTime?: number;
  placingField?: Field;
  lastPlacedStone?: Field;
  isQuittableByEveryone: boolean;
}

export default class Game extends Entity implements IGame {
  constructor(game: IGame) {
    super(game.id);
    this.match = game.match;
    this.activePlayerId = game.activePlayerId;
    this.board = game.board;
    this.moveStartTime = game.moveStartTime;
    this.gameStartTime = game.gameStartTime;
    this.placingField = game.placingField;
    this.lastPlacedStone = game.lastPlacedStone;
    this.isQuittableByEveryone = game.isQuittableByEveryone;
  }

  public match: Match;
  public activePlayerId: string;
  public board: string[][];
  public moveStartTime?: number;
  public gameStartTime?: number;
  public placingField?: Field;
  public lastPlacedStone?: Field;
  public isQuittableByEveryone: boolean;

  public playerLeft(identity: PlayerIdentity): InGamePlayer {
    return this.match.player2.id === identity.id ? this.match.player2 : this.match.player1;
  }
  public playerRight(identity: PlayerIdentity): InGamePlayer {
    return this.match.player2.id === identity.id ? this.match.player1 : this.match.player2;
  }
  public activePlayer(): InGamePlayer {
    return this.activePlayerId === this.match.player1.id ? this.match.player1 : this.match.player2;
  }
  public isParticipant(player: PlayerIdentity) {
    return player.id === this.match.player1.id || player.id === this.match.player2.id;
  }
}

import { Entity } from "./Entity";
import { PlayerIdentity } from "./PlayerIdentity";

export interface GameResultMatch extends Entity {
  player1: PlayerIdentity;
  player2: PlayerIdentity;
}

import { Entity } from "./Entity";
import { InGamePlayer } from "./InGamePlayer";

export interface Match extends Entity {
  player1: InGamePlayer;
  player2: InGamePlayer;
}

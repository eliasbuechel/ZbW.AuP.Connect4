import { IEntity } from "./Entity";
import { InGamePlayer } from "./InGamePlayer";

export interface Match extends IEntity {
  player1: InGamePlayer;
  player2: InGamePlayer;
}

import { PlayerIdentity } from "./PlayerIdentity";

export interface Match {
  id: string;
  player1: PlayerIdentity;
  player2: PlayerIdentity;
}

import { InGamePlayer } from "./InGamePlayer";

export interface InGameMatch {
  id: string;
  player1: InGamePlayer;
  player2: InGamePlayer;
}

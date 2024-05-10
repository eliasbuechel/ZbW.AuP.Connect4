import { InGameMatch } from "./InGameMatch";

export interface Game {
  match: InGameMatch;
  activePlayerId: string;
  connect4Board: string[][];
  startConfirmed: boolean;
}

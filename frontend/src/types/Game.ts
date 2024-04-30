import { Match } from "./Match";

export interface Game {
  match: Match;
  activePlayerId: string;
  connect4Board: string[][];
}

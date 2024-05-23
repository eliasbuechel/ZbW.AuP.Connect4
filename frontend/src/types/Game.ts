import { Entity } from "./Entity";
import { Match } from "./Match";

export interface Game extends Entity {
  match: Match;
  activePlayerId: string;
  connect4Board: string[][];
  startConfirmed: boolean;
}

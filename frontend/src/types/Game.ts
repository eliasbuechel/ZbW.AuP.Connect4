import { Entity } from "./Entity";
import { Field } from "./Field";
import { Match } from "./Match";

export interface Game extends Entity {
  match: Match;
  activePlayerId: string;
  connect4Board: string[][];
  moveStartTime?: number;
  gameStartTime?: number;
  placingField?: Field;
  lastPlacedStone?: Field;
}

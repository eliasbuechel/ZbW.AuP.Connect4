import { Entity } from "./Entity";
import { Field } from "./Field";
import { GameResultMatch } from "./GameResultMatch";

export interface GameResult extends Entity {
  winnerId?: string;
  line?: Field[];
  playedMoves: number[];
  startingPlayerId: string;
  match: GameResultMatch;
  // gameTimeInSeconds: number;
}

import { Entity } from "./Entity";
import { Field } from "./Field";
import { GameResultMatch } from "./GameResultMatch";
import { PlayedMove } from "./PlayedMove";

export interface GameResult extends Entity {
  winnerId?: string;
  line?: Field[];
  playedMoves: PlayedMove[];
  startingPlayerId: string;
  match: GameResultMatch;
  hasWinnerRow: boolean;
}

import { IEntity } from "./Entity";
import { Field } from "./Field";
import { GameResultMatch } from "./GameResultMatch";
import { PlayedMove } from "./PlayedMove";

export interface GameResult extends IEntity {
  winnerId?: string;
  line?: Field[];
  playedMoves: PlayedMove[];
  startingPlayerId: string;
  match: GameResultMatch;
}

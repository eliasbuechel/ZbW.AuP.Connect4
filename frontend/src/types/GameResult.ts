import { Connect4Line } from "./Connect4Line";
import { Match } from "./Match";

export interface GameResult {
  winnerId?: string;
  line?: Connect4Line;
  playedMoves: number[];
  startingPlayerId: string;
  match: Match;
}

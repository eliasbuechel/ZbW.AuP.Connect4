import { Connect4Line } from "./Connect4Line";

export interface GameResult {
  winnerId?: string;
  line?: Connect4Line;
}

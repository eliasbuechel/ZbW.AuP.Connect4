export interface Match {
  id: string;
  player1: PlayerIdentity;
  player2: PlayerIdentity;
}

export interface PlayerIdentity {
  id: string;
  username: string;
}

export interface OnlinePlayer extends PlayerIdentity {
  requestedMatch: boolean;
  youRequestedMatch: boolean;
  matched: boolean;
}

export interface Field {
  column: number;
  row: number;
}

export interface Connect4Line {
  line: Field[];
}

export interface GameResult {
  winnerId?: string;
  line?: Connect4Line;
}

export interface GameState {
  identity?: PlayerIdentity;
  game?: Game;
  gameResult?: GameResult;
  isSubscribed: boolean;
}

export interface Game {
  match: Match;
  activePlayerId: string;
  connect4Board: string[][];
}

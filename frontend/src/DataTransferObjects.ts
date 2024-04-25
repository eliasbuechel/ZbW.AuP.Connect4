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

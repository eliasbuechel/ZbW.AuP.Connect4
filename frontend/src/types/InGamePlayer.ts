import { PlayerIdentity } from "./PlayerIdentity";

export interface InGamePlayer extends PlayerIdentity {
  hasConfirmedGameStart: boolean;
  hintsLeft: number;
  currentHint?: number;
  totalPlayTime?: number;
}

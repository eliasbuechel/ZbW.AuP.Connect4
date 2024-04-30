import { PlayerIdentity } from "./PlayerIdentity";

export interface OnlinePlayer extends PlayerIdentity {
  requestedMatch: boolean;
  youRequestedMatch: boolean;
  matched: boolean;
}

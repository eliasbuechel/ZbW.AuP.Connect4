import { OnlinePlayer } from "./OnlinePlayer";

export interface ConnectedPlayers {
  webPlayers: OnlinePlayer[];
  opponentRoboterPlayers: OnlinePlayer[];
}

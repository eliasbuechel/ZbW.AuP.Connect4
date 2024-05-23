import { Entity } from "./Entity";

export interface Field extends Entity {
  column: number;
  row: number;
}

import { Entity } from "./Entity";

export interface PlayedMove extends Entity {
    column: number;
    duration: number;
}

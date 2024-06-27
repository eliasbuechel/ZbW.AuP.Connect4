﻿namespace backend.game
{
    internal class OpponentRoboterPlayer(string identification) : Player(Guid.NewGuid().ToString(), identification)
    {
        public bool IsHubPlayer { get; set; }
        public string Identification { get; } = identification;
    }
}
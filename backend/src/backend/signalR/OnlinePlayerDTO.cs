﻿using backend.game;

namespace backend.signalR
{
    internal class OnlinePlayerDTO : UserIdentityDTO
    {
        public OnlinePlayerDTO(IPlayer player, IPlayer you) : base(player)
        {
            RequestedMatch = player.HasRequestedMatch(you);
            YouRequestedMatch = you.HasRequestedMatch(player);
            Matched = you.HasMatched(player);
        }

        public bool RequestedMatch { get; }
        public bool YouRequestedMatch { get; }
        public bool Matched { get; }
    }
}

using backend.game;

namespace backend.communication.DOTs
{
    internal class ConnectedPlayerDTO : PlayerInfoDTO
    {
        public ConnectedPlayerDTO(Player player, Player potentialOpponent) : base(player)
        {
            RequestedMatch = potentialOpponent.MatchingRequests.Where(x => x.Player.Equals(player)).Any();
            YouRequestedMatch = player.MatchingRequests.Where(x => x.Player.Equals(potentialOpponent)).Any();
            Matched = player.Matching == potentialOpponent;
        }

        public ConnectedPlayerDTO(Player player) : base(player)
        {
            RequestedMatch = false;
            YouRequestedMatch = false;
            Matched = false;
        }

        public bool RequestedMatch { get; }
        public bool YouRequestedMatch { get; }
        public bool Matched { get; }
    }
}
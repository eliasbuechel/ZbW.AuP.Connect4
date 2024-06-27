using backend.game;

namespace backend.communication.dtos
{
    internal class ConnectedPlayerDto : PlayerInfoDto
    {
        public ConnectedPlayerDto(Player player, Player potentialOpponent) : base(player)
        {
            RequestedMatch = potentialOpponent.MatchingRequests.Any(x => x.Player.Equals(player));
            YouRequestedMatch = player.MatchingRequests.Any(x => x.Player.Equals(potentialOpponent));
            Matched = player.Matching == potentialOpponent;
        }

        public ConnectedPlayerDto(Player player) : base(player)
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
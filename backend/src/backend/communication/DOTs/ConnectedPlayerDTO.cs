using backend.game;

namespace backend.communication.DOTs
{
    internal class ConnectedPlayerDTO : PlayerInfoDTO
    {
        public ConnectedPlayerDTO(IPlayer player, IPlayer potentialOpponent) : base(player)
        {
            RequestedMatch = player.HasRequestedMatch(potentialOpponent);
            YouRequestedMatch = potentialOpponent.HasRequestedMatch(player);
            Matched = potentialOpponent.HasMatched(player);
        }

        public ConnectedPlayerDTO(IPlayer player) : base(player)
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
using backend.game.entities;

namespace backend.communication.DOTs
{
    internal class MatchDTO : EntityDTO
    {
        public MatchDTO(Match match) : base(match)
        {
            Player1 = new InGamePlayerDTO(match.Player1);
            Player2 = new InGamePlayerDTO(match.Player2);
        }

        public InGamePlayerDTO Player1 { get; }
        public InGamePlayerDTO Player2 { get; }
    }
}
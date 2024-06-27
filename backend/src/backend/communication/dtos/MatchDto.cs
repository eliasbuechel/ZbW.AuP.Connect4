using backend.game.entities;

namespace backend.communication.dtos
{
    internal class MatchDto(Match match) : EntityDto(match)
    {
        public InGamePlayerDto Player1 { get; } = new InGamePlayerDto(match.Player1);
        public InGamePlayerDto Player2 { get; } = new InGamePlayerDto(match.Player2);
    }
}
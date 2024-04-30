using backend.game;

namespace backend.communication.DOTs
{
    internal class MatchDTO
    {
        public MatchDTO(Match match)
        {
            Id = match.Id.ToString();
            Player1 = new PlayerIdentityDTO(match.Player1);
            Player2 = new PlayerIdentityDTO(match.Player2);
        }
        public string Id { get; }
        public PlayerIdentityDTO Player1 { get; }
        public PlayerIdentityDTO Player2 { get; }
    }
}

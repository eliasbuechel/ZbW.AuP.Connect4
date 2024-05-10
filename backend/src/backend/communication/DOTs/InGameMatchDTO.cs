using backend.game;

namespace backend.communication.DOTs
{
    internal class InGameMatchDTO
    {
        public InGameMatchDTO(Match match)
        {
            Id = match.Id.ToString();
            Player1 = new InGamePlayerDTO(match.Player1);
            Player2 = new InGamePlayerDTO(match.Player2);
        }
        public string Id { get; }
        public InGamePlayerDTO Player1 { get; }
        public InGamePlayerDTO Player2 { get; }
    }
}



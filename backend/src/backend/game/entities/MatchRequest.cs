namespace backend.game.entities
{
    internal class MatchRequest : Entity
    {
        public MatchRequest(Player requester, Player opponent)
        {
            Requester = requester;
            Opponent = opponent;
        }

        public Player Requester { get; }
        public Player Opponent { get; }
    }
}

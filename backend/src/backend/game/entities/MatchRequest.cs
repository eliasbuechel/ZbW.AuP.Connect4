namespace backend.game.entities
{
    internal class MatchRequest : Entity
    {
        public MatchRequest(IPlayer requester, IPlayer opponent)
        {
            Requester = requester;
            Opponent = opponent;
        }

        public IPlayer Requester { get; }
        public IPlayer Opponent { get; }
    }
}

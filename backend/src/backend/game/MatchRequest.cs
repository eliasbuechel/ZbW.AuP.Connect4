namespace backend.game
{
    internal class  MatchRequest
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

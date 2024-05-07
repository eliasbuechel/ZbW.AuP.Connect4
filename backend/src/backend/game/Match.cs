namespace backend.game
{
    internal class Match
    {
        public Match(MatchRequest request)
        {
            Player1 = request.Requester;
            Player2 = request.Opponent;
        }

        public Guid Id { get; } = Guid.NewGuid();
        public IPlayer Player1 { get; }
        public IPlayer Player2 { get; }
    }
}

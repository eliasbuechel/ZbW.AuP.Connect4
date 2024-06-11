namespace backend.game.entities
{
    internal class Match : Entity
    {
        public Match(MatchRequest matchRequest) : base(matchRequest)
        {
            Player1 = matchRequest.Requester;
            Player2 = matchRequest.Opponent;
        }
        public Match(Player player1, Player player2)
        {
            Player1 = player1;
            Player2 = player2;
        }

        public Player Player1 { get; }
        public Player Player2 { get; }
    }
}

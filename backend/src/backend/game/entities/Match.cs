namespace backend.game.entities
{
    internal class Match : Entity
    {
        public Match(Player player1, Player player2)
        {
            Player1 = player1;
            Player2 = player2;
        }

        public Player Player1 { get; }
        public Player Player2 { get; }
    }
}

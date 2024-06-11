namespace backend.game
{
    internal class AlgorythmPlayer : Player
    {
        public AlgorythmPlayer(Player opponentPlayer) : base(Guid.NewGuid().ToString(), "R4D4-Algorythm")
        {
            OpponentPlayer = opponentPlayer;
        }

        public Player OpponentPlayer { get; }
    }
}
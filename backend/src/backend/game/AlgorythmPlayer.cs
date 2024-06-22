namespace backend.game
{
    internal class AlgorythmPlayer(Player opponentPlayer) : Player(Guid.NewGuid().ToString(), "R4D4-Algorythm")
    {
        public Player OpponentPlayer { get; } = opponentPlayer;
    }
}
namespace backend.game
{
    internal class WrongPlayerPlayingMoveException : Exception
    {
        public WrongPlayerPlayingMoveException(string message, Player player) : base(message)
        {
            Player = player;
        }

        public Player Player { get; }
    }
}

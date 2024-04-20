namespace backend.game
{
    internal class WrongPlayerPlayingMoveException : Exception
    {
        public WrongPlayerPlayingMoveException(string message, IPlayer player) : base(message)
        {
            Player = player;
        }

        public IPlayer Player { get; }
    }
}

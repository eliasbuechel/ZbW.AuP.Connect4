namespace backend.game
{
    internal class MoveNotPossibleException : Exception
    {
        public MoveNotPossibleException(string message, Player player, int column) : base(message)
        {
            Player = player;
            Column = column;
        }

        public Player Player { get; }
        public int Column { get; }
    }
}

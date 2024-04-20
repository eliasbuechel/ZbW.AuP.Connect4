namespace backend.game
{
    internal class MoveNotPossibleException : Exception
    {
        public MoveNotPossibleException(string message, IPlayer player, int column) : base(message)
        {
            Player = player;
            Column = column;
        }

        public IPlayer Player { get; }
        public int Column { get; }
    }
}

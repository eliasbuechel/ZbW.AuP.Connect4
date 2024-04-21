namespace backend.game
{
    internal class PlayerManager : Exception
    {
        public PlayerManager(string message, IPlayer player) : base(message)
        {
            Player = player;
        }

        public IPlayer Player { get; }
    }
}

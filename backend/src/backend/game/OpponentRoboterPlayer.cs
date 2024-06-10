namespace backend.game
{
    internal class OpponentRoboterPlayer : Player
    {
        public OpponentRoboterPlayer(string identification) : base(Guid.NewGuid().ToString(), identification)
        { }
    }
}
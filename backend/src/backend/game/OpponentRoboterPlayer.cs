namespace backend.game
{
    internal class OpponentRoboterPlayer : Player
    {
        public OpponentRoboterPlayer(string identification) : base(Guid.NewGuid().ToString(), identification)
        { }
    }

    //internal class OpponentRoboterPlayer : Player
    //{
    //    public OpponentRoboterPlayer(string identification, AlgorythmPlayer opponentPlayer) : base(Guid.NewGuid().ToString(), identification)
    //    {
    //        OpponentPlayer = opponentPlayer;
    //    }

    //    public AlgorythmPlayer OpponentPlayer { get; }

    //    public override void ConfirmedGameStart(Player player)
    //    {
    //        throw new NotImplementedException();
    //    }
    //    public override void GameStarted(Game game)
    //    {
    //        throw new NotImplementedException();
    //    }
    //    public override void Matched(Match match)
    //    {
    //        throw new NotImplementedException();
    //    }
    //    public override void MovePlayed(Player player, Field field)
    //    {
    //        throw new NotImplementedException();
    //    }
    //    public override void RejectedMatch(Player player)
    //    {
    //        throw new NotImplementedException();
    //    }
    //    public override void RequestedMatch(Player player)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}

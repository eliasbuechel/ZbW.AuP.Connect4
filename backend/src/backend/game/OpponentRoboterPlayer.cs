namespace backend.game
{
    internal class OpponentRoboterPlayer : Player
    {
        public OpponentRoboterPlayer(string identification) : base(Guid.NewGuid().ToString(), identification)
        {
            Identification = identification;
        }

        public override bool HasConfirmedGameStart
        {
            get { return true; }
            set { }
        }

        public bool IsHubPlayer { get; set; }
        public string Identification { get; }
    }
}
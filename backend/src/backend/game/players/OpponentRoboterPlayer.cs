namespace backend.game.players
{
    internal class OpponentRoboterPlayer(string identification) : Player(Guid.NewGuid().ToString(), "Team 6")
    {
        public bool IsHubPlayer { get; set; }
        public string Identification { get; } = identification;
    }
}
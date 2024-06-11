using System.Security.Permissions;

namespace backend.game
{
    internal class OpponentRoboterPlayer : Player
    {
        public OpponentRoboterPlayer(string identification) : base(Guid.NewGuid().ToString(), identification)
        {
            Identification = identification;
        }

        public bool IsHubPlayer { get; set; }
        public string Identification { get; }
    }
}
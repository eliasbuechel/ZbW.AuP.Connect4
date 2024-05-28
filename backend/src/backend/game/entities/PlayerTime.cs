using backend.Data.entities;

namespace backend.game.entities
{
    internal class PlayerTime : Entity
    {
        public PlayerTime() { }
        public PlayerTime(DbPlayerTime playerMoveTime) : base(playerMoveTime)
        {
            TotalPlayerTime = playerMoveTime.TotalPlayerTime;
            PlayerMoveTimes = playerMoveTime.PlayerMoveTimes;
        }
        public double TotalPlayerTime { get; }
        public List<double> PlayerMoveTimes { get; }
    }
}

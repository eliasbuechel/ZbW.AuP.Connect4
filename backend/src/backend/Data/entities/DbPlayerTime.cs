using backend.game.entities;

namespace backend.Data.entities
{
    internal class DbPlayerTime : DbEntity
    {
        public DbPlayerTime() { }
        public DbPlayerTime(PlayerTime playerTime)
        {
            TotalPlayerTime = playerTime.TotalPlayerTime;
            PlayerMoveTimes = playerTime.PlayerMoveTimes;
        }

        public double TotalPlayerTime { get; set; }
        public List<double> PlayerMoveTimes { get; set; }
    }
}

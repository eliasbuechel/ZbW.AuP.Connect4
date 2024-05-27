using backend.game.entities;

namespace backend.communication.DOTs
{
    internal class PlayerTimeDTO : EntityDTO
    {
        public PlayerTimeDTO(PlayerTime playerTime) : base(playerTime)
        {
            TotalPlayerTime = playerTime.TotalPlayerTime;
            PlayerMoveTimes = playerTime.PlayerMoveTimes;
        }

        public double TotalPlayerTime { get; set; }
        public List<double> PlayerMoveTimes { get; set; }
    }
}

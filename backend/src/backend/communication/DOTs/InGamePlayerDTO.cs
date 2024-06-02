using backend.game;

namespace backend.communication.DOTs
{
    internal class InGamePlayerDTO : PlayerInfoDTO
    {
        public InGamePlayerDTO(IPlayer player) : base(player)
        {
            HasConfirmedGameStart = player.HasConfirmedGameStart;
            HintsLeft = player.HintsLeft;
            CurrentHint = player.CurrentHint;
            TotalPlayTime = Convert.ToInt64(player.TotalPlayTime.TotalMilliseconds);
        }

        public bool HasConfirmedGameStart { get; }
        public int HintsLeft { get; }
        public int? CurrentHint { get; }
        public long TotalPlayTime { get; }
    }
}
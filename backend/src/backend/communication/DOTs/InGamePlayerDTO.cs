using backend.game;

namespace backend.communication.DOTs
{
    internal class InGamePlayerDTO : PlayerInfoDTO
    {
        public InGamePlayerDTO(Player player) : base(player)
        {
            HasConfirmedGameStart = player.HasConfirmedGameStart;
            TotalPlayTime = player.TotalPlayTime == null ? null : Convert.ToInt64(player.TotalPlayTime.Value.TotalMilliseconds);

            if (player is WebPlayer webPlayer)
            {
                HintsLeft = webPlayer.HintsLeft;
                CurrentHint = webPlayer.CurrentHint;
            }
            else
            {
                HintsLeft = 0;
                CurrentHint = null;
            }
        }

        public bool HasConfirmedGameStart { get; }
        public int HintsLeft { get; }
        public int? CurrentHint { get; }
        public long? TotalPlayTime { get; }
    }
}
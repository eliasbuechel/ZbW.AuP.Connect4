using backend.game;

namespace backend.communication.DOTs
{
    internal class InGamePlayerDTO : PlayerInfoDTO
    {
        public InGamePlayerDTO(Player player) : base(player)
        {
            HasConfirmedGameStart = player.HasConfirmedGameStart;
            // TotalPlayTime = Convert.ToInt64(player.TotalPlayTime.TotalMilliseconds);

            if (player is WebPlayer webPlayer)
            {
                HintsLeft = webPlayer.HintsLeft;
                CurrentHint = webPlayer.CurrentHint;
            }

            HintsLeft = 0;
            CurrentHint = null;
        }

        public bool HasConfirmedGameStart { get; }
        public int HintsLeft { get; }
        public int? CurrentHint { get; }
        public long TotalPlayTime { get; }
    }
}
using backend.game;

namespace backend.communication.DOTs
{
    internal class InGamePlayerDTO : PlayerIdentityDTO
    {
        public InGamePlayerDTO(IPlayer player) : base(player)
        {
            HasConfirmedGameStart = player.HasConfirmedGameStart;
            HintsLeft = player.HintsLeft;
            CurrentHint = player.CurrentHint;
        }

        public bool HasConfirmedGameStart { get; }
        public int HintsLeft { get; }
        public int? CurrentHint { get; }
    }
}
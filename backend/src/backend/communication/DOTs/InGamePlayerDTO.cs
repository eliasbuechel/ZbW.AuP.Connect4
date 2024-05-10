using backend.game;

namespace backend.communication.DOTs
{
    internal class InGamePlayerDTO : PlayerIdentityDTO
    {
        public InGamePlayerDTO(IPlayer player) : base(player)
        {
            HasConfirmedGameStart = player.HasConfirmedGameStart;
        }

        public bool HasConfirmedGameStart { get; }
    }
}
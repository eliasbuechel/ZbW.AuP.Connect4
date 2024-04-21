using backend.game;

namespace backend.signalR
{
    internal class OnlinePlayerDTO : UserIdentityDTO
    {
        public OnlinePlayerDTO(IPlayer player) : base(player)
        {
            RequestedMatch = false;
        }

        public bool RequestedMatch { get; set; }
    }
}

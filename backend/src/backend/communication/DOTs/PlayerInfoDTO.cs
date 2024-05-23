using backend.game;
using backend.game.entities;

namespace backend.communication.DOTs
{
    internal class PlayerInfoDTO : EntityDTO
    {
        public PlayerInfoDTO(IPlayer player) : base(player.Id)
        {
            Username = player.Username;
        }
        public PlayerInfoDTO(PlayerInfo playerInfo) : base(playerInfo)
        {
            Username = playerInfo.Username;
        }

        public string Username { get; set; }
    }
}
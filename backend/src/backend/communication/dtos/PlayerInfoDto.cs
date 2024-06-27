using backend.game;
using backend.game.entities;

namespace backend.communication.dtos
{
    internal class PlayerInfoDto : EntityDto
    {
        public PlayerInfoDto(Player player) : base(player.Id)
        {
            Username = player.Username;
        }
        public PlayerInfoDto(PlayerInfo playerInfo) : base(playerInfo)
        {
            Username = playerInfo.Username;
        }

        public string Username { get; set; }
    }
}
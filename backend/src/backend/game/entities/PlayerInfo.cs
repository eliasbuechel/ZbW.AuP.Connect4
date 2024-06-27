using backend.data.entities;

namespace backend.game.entities
{
    internal class PlayerInfo : Entity
    {
        public PlayerInfo(Player player) : base(player.Id)
        {
            Username = player.Username;
        }
        public PlayerInfo(DbPlayerInfo playerInfo) : base(playerInfo)
        {
            Username = playerInfo.Username;
        }

        public string Username { get; }
    }
}
using backend.game;
using System.Diagnostics;

namespace backend.signalR
{
    internal class PlayerIdentityDTO
    {
        public PlayerIdentityDTO(IPlayer player)
        {
            Id = player.Id;
            string? username = player.Username;
            Debug.Assert(username != null);
            this.Username = username;
        }
        public string Id { get; set; }
        public string Username { get; set; }
    }
}

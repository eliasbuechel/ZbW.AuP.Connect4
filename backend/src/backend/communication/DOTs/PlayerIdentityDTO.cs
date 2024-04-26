using backend.game;
using System.Diagnostics;

namespace backend.communication.DOTs
{
    internal class PlayerIdentityDTO
    {
        public PlayerIdentityDTO(IPlayer player)
        {
            Id = player.Id;
            string? username = player.Username;
            Debug.Assert(username != null);
            Username = username;
        }
        public string Id { get; set; }
        public string Username { get; set; }
    }
}
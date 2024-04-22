
using backend.database;
using backend.signalR;
using Microsoft.AspNetCore.SignalR;
using System.Security.Principal;

namespace backend.game
{
    internal class SignalRPlayer : Player
    {
        public SignalRPlayer(PlayerIdentity identity, GameManager gameManager, backend.services.PlayerManager playerManager, IHubContext<SignalRPlayerHub> playerHubContext) : base(identity, gameManager, playerManager)
        {
            _playerHubContext = playerHubContext;
        }

        public override async void PlayerConnected(IPlayer player)
        {
            OnlinePlayerDTO onlinePlayer = new OnlinePlayerDTO(player);
            foreach (string connection in Connections)
                await _playerHubContext.Clients.Client(connection).SendAsync("player-connected", onlinePlayer);
        }
        public override async void PlayerDisconnected(IPlayer player)
        {
            foreach (string connection in Connections)
                await _playerHubContext.Clients.Client(connection).SendAsync("player-disconnected", player.Id);
        }
        public override async void RequestedMatch(IPlayer player)
        {
            foreach (string connection in Connections)
                await _playerHubContext.Clients.Client(connection).SendAsync("requested-game", player.Id);
        }
        public override async void DeclineMatch(IPlayer player)
        {
            foreach (string connection in Connections)
                await _playerHubContext.Clients.Client(connection).SendAsync("decline-game", player.Id);
        }

        private readonly IHubContext<SignalRPlayerHub> _playerHubContext;
    }
}

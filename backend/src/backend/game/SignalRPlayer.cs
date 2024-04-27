
using backend.Data;
using backend.signalR;
using Microsoft.AspNetCore.SignalR;
using System.Numerics;
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
            OnlinePlayerDTO onlinePlayer = new OnlinePlayerDTO(player, this);
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
                await _playerHubContext.Clients.Client(connection).SendAsync("player-requested-match", player.Id);
        }
        public override async void RejectedMatch(IPlayer player)
        {
            foreach (string connection in Connections)
                await _playerHubContext.Clients.Client(connection).SendAsync("player-rejected-match", player.Id);
        }
        public override async void Matched(IPlayer player)
        {
            foreach (string connection in Connections)
                await _playerHubContext.Clients.Client(connection).SendAsync("matched", player.Id);

        }

        private readonly IHubContext<SignalRPlayerHub> _playerHubContext;
    }
}

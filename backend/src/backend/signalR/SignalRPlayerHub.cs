using backend.database;
using backend.game;
using backend.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;

namespace backend.signalR
{
    [Authorize]
    internal class SignalRPlayerHub : Hub 
    {
        public SignalRPlayerHub(services.PlayerManager playerManager, GameManager gameManager, UserManager<PlayerIdentity> userManager, IHubContext<SignalRPlayerHub> playerHubContext)
        {
            _playerManager = playerManager;
            _gameManager = gameManager;
            _userManager = userManager;
            _playerHubContext = playerHubContext;
        }

        public async void GetOnlinePlayers()
        {
            IEnumerable<IPlayer> onlinePlayers = CurrentPlayer.GetOnlinePlayers();
            IEnumerable<OnlinePlayerDTO> onlinePlayersDTO = onlinePlayers.Select(p => new OnlinePlayerDTO(p)).ToArray();
            string data = JsonSerializer.Serialize(onlinePlayersDTO);
            await Clients.Caller.SendAsync("send-online-players", data);
        }
        public async void GetUserData()
        {
            UserIdentityDTO userData = new UserIdentityDTO(CurrentPlayer);
            string data = JsonSerializer.Serialize(userData);
            await Clients.Caller.SendAsync("send-user-data", data);
        }
        public void RequestMatch(string playerId)
        {
            IPlayer player = _playerManager.GetPlayer(playerId);
            CurrentPlayer.RequestMatch(player);
        }

        //public void MakeMove(int column)
        //{
        //    PlayerIdentity identity = GetIdentity();
        //    IPlayer player = _playerManager.GetPlayer(identity);
        //    player.MakeMove(column);
        //}

        public override Task OnConnectedAsync()
        {
            IPlayer? player = _playerManager.GetPlayerOrDefault(Identity);
            if (player == null)
                player = new SignalRPlayer(Identity, _gameManager, _playerManager, _playerHubContext);

            player.Connected();
            return Task.CompletedTask;
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            CurrentPlayer.Disconnected();
            return Task.CompletedTask;
        }

        private PlayerIdentity Identity
        {
            get
            {
                ClaimsPrincipal? claimsPrincipal = Context.User;
                Debug.Assert(claimsPrincipal != null);

                string? userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
                Debug.Assert(userId != null);

                PlayerIdentity? identity = _userManager.FindByIdAsync(userId).Result;
                Debug.Assert(identity != null);

                return identity;
            }
        }
        private IPlayer CurrentPlayer => _playerManager.GetPlayer(Identity);

        private readonly services.PlayerManager _playerManager;
        private readonly GameManager _gameManager;
        private readonly UserManager<PlayerIdentity> _userManager;
        private readonly IHubContext<SignalRPlayerHub> _playerHubContext;
    }
}

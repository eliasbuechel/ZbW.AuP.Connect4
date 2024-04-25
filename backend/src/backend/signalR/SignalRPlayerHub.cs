using backend.database;
using backend.game;
using backend.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;

namespace backend.signalR
{
    [Authorize]
    internal class SignalRPlayerHub : Hub 
    {
        public SignalRPlayerHub(services.PlayerManager playerManager, GameManager gameManager, UserManager<PlayerIdentity> userManager, IHubContext<SignalRPlayerHub> playerHubContext, PlayerRequestLock playerRequestLock)
        {
            _playerManager = playerManager;
            _gameManager = gameManager;
            _userManager = userManager;
            _playerHubContext = playerHubContext;
            _playerRequestLock = playerRequestLock;
        }

        public void GetOnlinePlayers()
        {
            lock (_playerRequestLock[Identity])
            {
                IEnumerable<IPlayer> onlinePlayers = CurrentPlayer.GetOnlinePlayers();
                IEnumerable<OnlinePlayerDTO> onlinePlayersDTO = onlinePlayers.Select(p => new OnlinePlayerDTO(p, CurrentPlayer)).ToArray();
                Clients.Caller.SendAsync("send-online-players", onlinePlayersDTO).Wait();
            }
        }
        public void GetUserData()
        {
            lock (_playerRequestLock[Identity])
            {
                UserIdentityDTO userData = new UserIdentityDTO(CurrentPlayer);
                Clients.Caller.SendAsync("send-user-data", userData).Wait();
            }
        }
        public void RequestMatch(string playerId)
        {
            lock (_playerRequestLock[Identity])
            {
                IPlayer player = _playerManager.GetPlayer(playerId);
                CurrentPlayer.RequestMatch(player);
            }
        }
        public void AcceptMatch(string playerId)
        {
            lock (_playerRequestLock[Identity])
            {
                IPlayer player = _playerManager.GetPlayer(playerId);
                CurrentPlayer.AcceptMatch(player);
            }
        }
        public void RejectMatch(string playerId)
        {
            lock (_playerRequestLock[Identity])
            {
                IPlayer player = _playerManager.GetPlayer(playerId);
                CurrentPlayer.RejectMatch(player);
            }
        }

        public override Task OnConnectedAsync()
        {
            lock (_playerRequestLock[Identity])
            {
                IPlayer? player = _playerManager.GetPlayerOrDefault(Identity);
                if (player == null)
                    player = new SignalRPlayer(Identity, _gameManager, _playerManager, _playerHubContext);

                player.Connected(Context.ConnectionId);
                return Task.CompletedTask;
            }
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            lock (_playerRequestLock[Identity])
            {
                IPlayer? player = _playerManager.GetPlayerOrDefault(Identity);

                if (player != null)
                    CurrentPlayer.Disconnected(Context.ConnectionId);

                return Task.CompletedTask;
            }
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
        private readonly PlayerRequestLock _playerRequestLock;
    }
}

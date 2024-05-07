using backend.communication.DOTs;
using backend.Data;
using backend.game;
using backend.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Security.Claims;

namespace backend.communication.signalR
{
    [Authorize]
    internal class PlayerHub : Hub
    {
        public PlayerHub(IOnlinePlayerProvider onlinePlayerProvider, UserManager<PlayerIdentity> userManager, Func<PlayerIdentity, ToPlayerHub<PlayerHub>> createPlayer, PlayerRequestLock playerRequestLock)
        {
            _onlinePlayerProvider = onlinePlayerProvider;
            _userManager = userManager;
            _createPlayer = createPlayer;
            _playerRequestLock = playerRequestLock;
        }

        public void GetGamePlan()
        {
            lock (_playerRequestLock[Identity])
            {
                ThisPlayer.GetGamePlan(Connection).Wait();
            }
        }
        public void GetGame()
        {
            lock (_playerRequestLock[Identity])
            {
                ThisPlayer.GetGame(Connection).Wait();
            }
        }
        public void GetUserData()
        {
            lock (_playerRequestLock[Identity])
            {
                ThisPlayer.GetUserDataAsync(Connection).Wait();
            }
        }
        public void GetOnlinePlayers()
        {
            lock (_playerRequestLock[Identity])
            {
                ThisPlayer.GetOnlinePlayers(Connection).Wait();
            }
        }
        public void GetCurrentGame()
        {
            lock (_playerRequestLock[Identity])
            {
                ThisPlayer.GetCurrentGame(Connection).Wait();
            }
        }
        public void RequestMatch(string playerId)
        {
            lock (_playerRequestLock[Identity])
            {
                IPlayer player = _onlinePlayerProvider.GetPlayer(playerId);
                ThisPlayer.RequestMatch(player);
            }
        }
        public void AcceptMatch(string playerId)
        {
            lock (_playerRequestLock[Identity])
            {
                IPlayer player = _onlinePlayerProvider.GetPlayer(playerId);
                ThisPlayer.AcceptMatch(player);
            }
        }
        public void RejectMatch(string playerId)
        {
            lock (_playerRequestLock[Identity])
            {
                IPlayer player = _onlinePlayerProvider.GetPlayer(playerId);
                ThisPlayer.RejectMatch(player).Wait();
            }
        }
        public void PlayMove(int column)
        {
            lock (_playerRequestLock[Identity])
            {
                ThisPlayer.PlayMove(column);
            }
        }
        public void QuitGame()
        {
            lock (_playerRequestLock[Identity])
            {
                ThisPlayer.QuitGame();
            }
        }

        public override Task OnConnectedAsync()
        {
            lock (_playerRequestLock[Identity])
            {
                IPlayer? player = _onlinePlayerProvider.GetPlayerOrDefault(Identity);
                if (player == null)
                    player = _createPlayer(Identity);

                player.Connect(Connection);
                return Task.CompletedTask;
            }
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            lock (_playerRequestLock[Identity])
            {
                IPlayer? player = _onlinePlayerProvider.GetPlayerOrDefault(Identity);

                if (player != null)
                    ThisPlayer.Disconnected(Connection);

                return Task.CompletedTask;
            }
        }

        private string Connection => Context.ConnectionId;
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
        private IPlayer ThisPlayer => _onlinePlayerProvider.GetPlayer(Identity);

        private readonly IOnlinePlayerProvider _onlinePlayerProvider;
        private readonly PlayerRequestLock _playerRequestLock;
        private readonly UserManager<PlayerIdentity> _userManager;
        private readonly Func<PlayerIdentity, ToPlayerHub<PlayerHub>> _createPlayer;
    }
}

using backend.communication.DOTs;
using backend.database;
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
        public PlayerHub(IOnlinePlayerProvider onlinePlayerProvider, UserManager<PlayerIdentity> userManager, Func<PlayerIdentity, HubPlayer<PlayerHub>> createPlayer, PlayerRequestLock playerRequestLock)
        {
            _onlinePlayerProvider = onlinePlayerProvider;
            _userManager = userManager;
            _createPlayer = createPlayer;
            _playerRequestLock = playerRequestLock;
        }

        public void GetUserData()
        {
            lock (_playerRequestLock[Identity])
            {
                PlayerIdentityDTO userData = new PlayerIdentityDTO(ThisPlayer);
                Clients.Caller.SendAsync("send-user-data", userData).Wait();
            }
        }
        public void GetOnlinePlayers()
        {
            lock (_playerRequestLock[Identity])
            {
                IEnumerable<IPlayer> onlinePlayers = ThisPlayer.GetOnlinePlayers();
                IEnumerable<OnlinePlayerDTO> onlinePlayersDTO = onlinePlayers.Select(p => new OnlinePlayerDTO(p, ThisPlayer)).ToArray();
                Clients.Caller.SendAsync("send-online-players", onlinePlayersDTO).Wait();
            }
        }
        public void GetGamePlan()
        {
            lock (_playerRequestLock[Identity])
            {
                IEnumerable<Match> gamePlan = ThisPlayer.GetGamePlan();
                IEnumerable<MatchDTO> gamePlanDTO = gamePlan.Select(m => new MatchDTO(m)).ToList();
                Clients.Caller.SendAsync("send-game-plan", gamePlanDTO).Wait();
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
                ThisPlayer.RejectMatch(player);
            }
        }

        public override Task OnConnectedAsync()
        {
            lock (_playerRequestLock[Identity])
            {
                IPlayer? player = _onlinePlayerProvider.GetPlayerOrDefault(Identity);
                if (player == null)
                    player = _createPlayer(Identity);
                //new HubPlayer(Identity, _gameManager, _playerHubContext);

                player.Connected(Context.ConnectionId);
                return Task.CompletedTask;
            }
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            lock (_playerRequestLock[Identity])
            {
                IPlayer? player = _onlinePlayerProvider.GetPlayerOrDefault(Identity);

                if (player != null)
                    ThisPlayer.Disconnected(Context.ConnectionId);

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
        private IPlayer ThisPlayer => _onlinePlayerProvider.GetPlayer(Identity);

        private readonly IOnlinePlayerProvider _onlinePlayerProvider;
        private readonly PlayerRequestLock _playerRequestLock;
        private readonly UserManager<PlayerIdentity> _userManager;
        private readonly Func<PlayerIdentity, HubPlayer<PlayerHub>> _createPlayer;
    }
}

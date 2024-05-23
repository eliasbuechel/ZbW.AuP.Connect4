using backend.Data;
using backend.game;
using backend.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using System.Security.Claims;

namespace backend.communication.signalR
{
    [Authorize]
    internal class WebPlayerHub : PlayerHub
    {
        public WebPlayerHub(IOnlinePlayerProvider onlinePlayerProvider, AlgorythmPlayerProvider algorythmPlayerProvider, UserManager<PlayerIdentity> userManager, Func<PlayerIdentity, ToPlayerHub<WebPlayerHub>> createPlayer, PlayerRequestLock playerRequestLock) : base(onlinePlayerProvider)
        {
            _userManager = userManager;
            _createPlayer = createPlayer;
            _playerRequestLock = playerRequestLock;
            _algorythmPlayerProvider = algorythmPlayerProvider;
        }
        public void RequestSinglePlayerMatch()
        {
            lock (RequestLock)
            {
                IPlayer algorythmPlayer = _algorythmPlayerProvider.CreateAlgorythmPlayer();
                ThisPlayer.RequestMatch(algorythmPlayer);
            }
        }
        public void GetHint()
        {
            lock (RequestLock)
            {
                ThisPlayer.GetHint();
            }
        }

        protected override IPlayer ThisPlayer => _onlinePlayerProvider.GetOnlinePlayer(Identity.Id);
        protected override object RequestLock => _playerRequestLock[Identity];
        protected override IPlayer GetOrCreatePlayer()
        {
            IPlayer? player = _onlinePlayerProvider.GetOnlinePlayerOrDefault(Identity.Id);
            if (player != null)
                return player;

            return _createPlayer(Identity);
        }
        protected override IPlayer? GetPlayerOrDefault()
        {
            return _onlinePlayerProvider.GetOnlinePlayerOrDefault(Identity.Id);
        }
        public void GetBestlist()
        {
            lock (RequestLock)
            {
                ThisPlayer.GetBestlist(Connection);
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

        private readonly PlayerRequestLock _playerRequestLock;
        private readonly AlgorythmPlayerProvider _algorythmPlayerProvider;
        private readonly UserManager<PlayerIdentity> _userManager;
        private readonly Func<PlayerIdentity, ToPlayerHub<WebPlayerHub>> _createPlayer;
    }
}

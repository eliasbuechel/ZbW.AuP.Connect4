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
        public WebPlayerHub(IOnlinePlayerProvider onlinePlayerProvider, PlayerRequestHandlerManager playerRequestHandlerManager, AlgorythmPlayerProvider algorythmPlayerProvider, UserManager<PlayerIdentity> userManager, Func<PlayerIdentity, ToPlayerHub<WebPlayerHub>> createPlayer) : base(onlinePlayerProvider, playerRequestHandlerManager)
        {
            _userManager = userManager;
            _createPlayer = createPlayer;
            _algorythmPlayerProvider = algorythmPlayerProvider;
        }
        public void RequestSinglePlayerMatch()
        {
            IPlayer player;
            try
            {
                player = ThisPlayer;
                IPlayer algorythmPlayer = _algorythmPlayerProvider.CreateAlgorythmPlayer();
                RequestHandler.Enqueue(() => player.RequestMatch(algorythmPlayer));
            }
            catch
            {
                Debug.Assert(false);
            }
        }
        public void GetHint()
        {
            IPlayer player;
            try
            {
                player = ThisPlayer;
                RequestHandler.Enqueue(() =>
                {
                    player.GetHint();
                    return Task.CompletedTask;
                });
            }
            catch
            {
                Debug.Assert(false);
            }
        }
        public void GetBestlist()
        {
            ThisPlayer.GetBestlist(Connection);
        }

        protected override IPlayer ThisPlayer => _onlinePlayerProvider.GetOnlinePlayerAsync(Identity.Id);

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

        private readonly AlgorythmPlayerProvider _algorythmPlayerProvider;
        private readonly UserManager<PlayerIdentity> _userManager;
        private readonly Func<PlayerIdentity, ToPlayerHub<WebPlayerHub>> _createPlayer;
    }
}

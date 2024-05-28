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
        public WebPlayerHub(PlayerRequestHandlerManager playerRequestHandlerManager, IOnlinePlayerProvider onlinePlayerProvider, AlgorythmPlayerProvider algorythmPlayerProvider, UserManager<PlayerIdentity> userManager, Func<AlgorythmPlayer> createAlgorythmPlayer, Func<PlayerIdentity, ToPlayerHub<WebPlayerHub>> createWebPlayer) : base(playerRequestHandlerManager)
        {
            _userManager = userManager;
            _createAlgorythmPlayer = createAlgorythmPlayer;
            _createWebPlayer = createWebPlayer;
            _algorythmPlayerProvider = algorythmPlayerProvider;
            _onlinePlayerProvider = onlinePlayerProvider;
        }

        public void RequestMatch(string playerId)
        {
            IPlayer player;
            try
            {
                player = ThisPlayer;
                RequestHandler.Enqueue(async () =>
                {
                    IPlayer opponent = _onlinePlayerProvider.GetOnlinePlayerAsync(playerId);
                    await player.RequestMatch(opponent);
                });
            }
            catch
            {
                Debug.Assert(false);
            }
        }
        public void AcceptMatch(string playerId)
        {
            IPlayer player;
            try
            {
                player = ThisPlayer;
                RequestHandler.Enqueue(async () =>
                {
                    IPlayer opponent = _onlinePlayerProvider.GetOnlinePlayerAsync(playerId);
                    await player.AcceptMatchAsync(opponent);
                });
            }
            catch
            {
                Debug.Assert(false);
            }
        }
        public void RejectMatch(string playerId)
        {
            IPlayer player;
            try
            {
                player = ThisPlayer;
                RequestHandler.Enqueue(async () =>
                {
                    IPlayer opponent = _onlinePlayerProvider.GetOnlinePlayerAsync(playerId);
                    await player.RejectMatchAsync(opponent);
                });
            }
            catch
            {
                Debug.Assert(false);
            }
        }
        public void RequestSinglePlayerMatch()
        {
            IPlayer player;
            try
            {
                player = ThisPlayer;
                IPlayer algorythmPlayer = _algorythmPlayerProvider.CreateAlgorythmPlayer(player, _createAlgorythmPlayer);
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
            return _onlinePlayerProvider.GetOrCreatePlayer(Identity, _createWebPlayer);
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

        private readonly UserManager<PlayerIdentity> _userManager;
        private readonly Func<AlgorythmPlayer> _createAlgorythmPlayer;
        private readonly IOnlinePlayerProvider _onlinePlayerProvider;
        private readonly AlgorythmPlayerProvider _algorythmPlayerProvider;
        private readonly Func<PlayerIdentity, ToPlayerHub<WebPlayerHub>> _createWebPlayer;
    }
}
using backend.Data;
using backend.game;
using backend.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace backend.communication.signalR
{
    [Authorize]
    internal class WebPlayerHub : PlayerHub<WebPlayer, PlayerIdentity, IPlayer>
    {
        public WebPlayerHub(
            PlayerRequestHandlerManager playerRequestHandlerManager,
            WebPlayerManager webPlayerManager,
            Func<PlayerIdentity, WebPlayer> createWebPlayer,
            UserManager<PlayerIdentity> userManager,
            OpponentRoboterPlayerHubClientManager opponentRoboterPlayerHubClientManager,
            AlgorythmPlayerManager algorythmPlayerManager,
            Func<IPlayer, AlgorythmPlayer> createAlgorythmPlayer,
            Func<string, OpponentRoboterPlayerHubClient> createOpponentRoboterPlayer,
            ConnectedPlayerProvider connectedPlayerProvider
            ) : base(playerRequestHandlerManager, webPlayerManager, createWebPlayer, connectedPlayerProvider)
        {
            _userManager = userManager;
            _algorythmPlayerManager = algorythmPlayerManager;
            _opponentRoboterPlayerHubClientManager = opponentRoboterPlayerHubClientManager;
            _createAlgorythmPlayer = createAlgorythmPlayer;
            _createOpponentRoboterPlayer = createOpponentRoboterPlayer;
        }

        public void RequestMatch(string playerId)
        {
            IPlayer player;
            try
            {
                player = ThisPlayer;

                RequestHandler.Enqueue(async () =>
                {
                    IPlayer? opponent = _connectedPlayerProvider.GetPlayer(playerId);

                    if (opponent is WebPlayer)
                    {
                        if (opponent == null)
                        {
                            Debug.Assert(false);
                            return;
                        }

                        await player.RequestMatch(opponent);
                    }
                    else if (opponent is OpponentRoboterPlayer opponentRoboterPlayer)
                    {
                        AlgorythmPlayer algorythmPlayer = _algorythmPlayerManager.ConnectPlayer(opponentRoboterPlayer, _createAlgorythmPlayer);
                        await algorythmPlayer.RequestMatch(opponentRoboterPlayer);
                    }
                    else if (opponent is OpponentRoboterPlayerHubClient opponentRoboterPlayerHubClient)
                    {
                        AlgorythmPlayer algorythmPlayer = _algorythmPlayerManager.ConnectPlayer(opponentRoboterPlayerHubClient, _createAlgorythmPlayer);
                        await algorythmPlayer.RequestMatch(opponentRoboterPlayerHubClient);
                    }
                    else
                    {
                        Debug.Assert(false);
                    }
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
                    IPlayer? opponent = _connectedPlayerProvider.GetPlayer(playerId);
                    
                    if (opponent is OpponentRoboterPlayer || opponent is OpponentRoboterPlayerHubClient)
                    {
                        AlgorythmPlayer algorythmPlayer = _algorythmPlayerManager.GetConnectedPlayerByIdentification(opponent);
                        await algorythmPlayer.AcceptMatch(opponent);
                        return;
                    }
                    
                    if (opponent == null)
                    {
                        Debug.Assert(false);
                        return;
                    }

                    await player.AcceptMatch(opponent);
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
                    IPlayer? opponent = _connectedPlayerProvider.GetPlayer(playerId);
                    if (opponent == null)
                    {
                        Debug.Assert(false);
                        return;
                    }

                    await player.RejectMatch(opponent);
                });
            }
            catch
            {
                Debug.Assert(false);
            }
        }
        public void RequestSinglePlayerMatch()
        {
            ToPlayerHub<WebPlayerHub> player;
            try
            {
                player = ThisPlayer;
                _algorythmPlayerManager.ConnectPlayer(player, _createAlgorythmPlayer);
                RequestHandler.Enqueue(async () =>
                {
                    _algorythmPlayerManager.ConnectPlayer(player, _createAlgorythmPlayer);
                    AlgorythmPlayer algorythmPlayer = _algorythmPlayerManager.GetConnectedPlayerByIdentification(player);
                    await player.RequestMatch(algorythmPlayer);
                });
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
        public async void GetBestlist()
        {
            await ThisPlayer.GetBestlist(Connection);
        }
        public void ConnectToOpponentRoboterPlayer(string hubUrl)
        {
            try
            {
                RequestHandler.Enqueue(() =>
                {
                    _opponentRoboterPlayerHubClientManager.ConnectPlayer(hubUrl, _createOpponentRoboterPlayer);
                    OpponentRoboterPlayerHubClient opponentRoboterPlayer = _opponentRoboterPlayerHubClientManager.GetConnectedPlayerByIdentification(hubUrl);
                    return Task.CompletedTask;
                });
            }
            catch
            {
                Debug.Assert(false);
            }
        }
        public void WatchGame()
        {
            var player = ThisPlayer;
            Debug.Assert(player != null);

            RequestHandler.Enqueue(() =>
            {
                player.WatchGame();
                return Task.CompletedTask;
            });
        }
        public void StopWatchingGame()
        {
            var player = ThisPlayer;
            Debug.Assert(player != null);

            RequestHandler.Enqueue(() =>
            {
                player.StopWatchingGame();
                return Task.CompletedTask;
            });
        }

        protected override PlayerIdentity Identification
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
        private readonly AlgorythmPlayerManager _algorythmPlayerManager;
        private readonly OpponentRoboterPlayerHubClientManager _opponentRoboterPlayerHubClientManager;
        private readonly Func<IPlayer, AlgorythmPlayer> _createAlgorythmPlayer;
        private readonly Func<string, OpponentRoboterPlayerHubClient> _createOpponentRoboterPlayer;
    }
}
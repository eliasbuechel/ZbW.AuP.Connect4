using backend.game;
using backend.services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.OpenApi.Expressions;
using Org.BouncyCastle.Asn1.X509.Qualified;
using System.Diagnostics;

namespace backend.communication.signalR
{
    internal abstract class PlayerHub : Hub
    {
        public PlayerHub(IOnlinePlayerProvider onlinePlayerProvider, PlayerRequestHandlerManager playerRequestHandlerManager)
        {
            _onlinePlayerProvider = onlinePlayerProvider;
            _playerRequestHandlerManager = playerRequestHandlerManager;
        }

        public void GetGamePlan()
        {
            lock (RequestLock)
            {
                IPlayer player;
                try
                {
                    player = ThisPlayer;
                    string connection = Connection;
                    RequestHandler.Enqueue(() => player.GetGamePlanAsync(connection));
                }
                catch
                {
                    Debug.Assert(false);
                }
            }
        }
        public void GetGame()
        {
            lock (RequestLock)
            {
                IPlayer player;
                try
                {
                    player = ThisPlayer;
                    string connection = Connection;
                    RequestHandler.Enqueue(() => player.GetGameAsync(connection));
                }
                catch
                {
                    Debug.Assert(false);
                }
            }
        }
        public void GetUserData()
        {
            lock (RequestLock)
            {
                IPlayer player;
                try
                {
                    player = ThisPlayer;
                    string connection = Connection;
                    RequestHandler.Enqueue(() => player.GetUserDataAsync(connection));
                }
                catch
                {
                    Debug.Assert(false);
                }
            }
        }
        public void GetOnlinePlayers()
        {
            lock (RequestLock)
            {
                IPlayer player;
                try
                {
                    player = ThisPlayer;
                    string connection = Connection;
                    RequestHandler.Enqueue(() => player.GetOnlinePlayersAsync(connection));
                }
                catch
                {
                    Debug.Assert(false);
                }
            }
        }
        public void GetCurrentGame()
        {
            lock (RequestLock)
            {
                IPlayer player;
                try
                {
                    player = ThisPlayer;
                    string connection = Connection;
                    RequestHandler.Enqueue(() => player.GetCurrentGameAsync(connection));
                }
                catch
                {
                    Debug.Assert(false);
                }
            }
        }
        public void RequestMatch(string playerId)
        {
            lock (RequestLock)
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
        }
        public void AcceptMatch(string playerId)
        {
            lock (RequestLock)
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
        }
        public void RejectMatch(string playerId)
        {
            lock (RequestLock)
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
        }
        public void ConfirmGameStart()
        {
            lock (RequestLock)
            {
                IPlayer player;
                try
                {
                    player = ThisPlayer;
                    RequestHandler.Enqueue(player.ConfirmGameStartAsync);
                }
                catch
                {
                    Debug.Assert(false);
                }
            }
        }
        public void PlayMove(int column)
        {
            lock (RequestLock)
            {
                IPlayer player;
                try
                {
                    player = ThisPlayer;
                    RequestHandler.Enqueue(() => player.PlayMoveAsync(column));
                }
                catch
                {
                    Debug.Assert(false);
                }
            }
        }
        public void QuitGame()
        {
            lock (RequestLock)
            {
                IPlayer player;
                try
                {
                    player = ThisPlayer;
                    RequestHandler.Enqueue(player.QuitGameAsync);
                }
                catch
                {
                    Debug.Assert(false);
                }
            }
        }

        public override Task OnConnectedAsync()
        {
            lock (RequestLock)
            {
                IPlayer player = GetOrCreatePlayer();
                string connection = Connection;
                PlayerRequestHandler requestHandler = _playerRequestHandlerManager.GetOrCreateHandler(player);

                requestHandler.Enqueue(() =>
                {
                    player.ConnectAsync(connection);
                    return Task.CompletedTask;
                });
            }

            return Task.CompletedTask;
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            lock (RequestLock)
            {
                string connection = Connection;
                IPlayer? player = GetPlayerOrDefault();

                RequestHandler.Enqueue(() =>
                {
                    if (player != null)
                        player.Disconnected(connection);

                    return Task.CompletedTask;
                });

            }

            return Task.CompletedTask;
        }

        protected abstract IPlayer GetOrCreatePlayer();
        protected abstract IPlayer? GetPlayerOrDefault();

        protected string Connection => Context.ConnectionId;
        protected abstract IPlayer ThisPlayer { get; }
        protected abstract object RequestLock { get; }
        protected PlayerRequestHandler RequestHandler => _playerRequestHandlerManager.GetOrCreateHandler(ThisPlayer);

        protected readonly IOnlinePlayerProvider _onlinePlayerProvider;
        private readonly PlayerRequestHandlerManager _playerRequestHandlerManager;
    }
}

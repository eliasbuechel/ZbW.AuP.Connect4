using backend.game;
using backend.services;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace backend.communication.signalR
{
    internal abstract class PlayerHub : Hub
    {
        public PlayerHub(PlayerRequestHandlerManager playerRequestHandlerManager)
        {
            _playerRequestHandlerManager = playerRequestHandlerManager;
        }

        public void GetGamePlan()
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
        public void GetGame()
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
        public void GetUserData()
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
        public void GetOnlinePlayers()
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
        public void GetCurrentGame()
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
        public void ConfirmGameStart()
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
        public void PlayMove(int column)
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
        public void QuitGame()
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

        public override Task OnConnectedAsync()
        {
            IPlayer player = GetOrCreatePlayer();
            string connection = Connection;
            PlayerRequestHandler requestHandler = _playerRequestHandlerManager.GetOrCreateHandler(player);

            requestHandler.Enqueue(() =>
            {
                player.ConnectAsync(connection);
                return Task.CompletedTask;
            });

            return Task.CompletedTask;
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            string connection = Connection;
            IPlayer? player = GetPlayerOrDefault();

            RequestHandler.Enqueue(() =>
            {
                if (player != null)
                    player.Disconnected(connection);

                return Task.CompletedTask;
            });

            return Task.CompletedTask;
        }

        protected abstract IPlayer GetOrCreatePlayer();
        protected abstract IPlayer? GetPlayerOrDefault();

        protected string Connection => Context.ConnectionId;
        protected abstract IPlayer ThisPlayer { get; }
        protected PlayerRequestHandler RequestHandler => _playerRequestHandlerManager.GetOrCreateHandler(ThisPlayer);

        private readonly PlayerRequestHandlerManager _playerRequestHandlerManager;
    }
}

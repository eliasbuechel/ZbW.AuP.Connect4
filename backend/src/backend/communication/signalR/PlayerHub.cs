using backend.game;
using backend.services;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace backend.communication.signalR
{
    internal abstract class PlayerHub<TPlayer, TIdentification> : Hub where TPlayer : IPlayer
    {
        public PlayerHub(PlayerRequestHandlerManager playerRequestHandlerManager, PlayerManager<TPlayer, TIdentification> playerManager, Func<TIdentification, TPlayer> createPlayer, ConnectedPlayerProvider connectedPlayerProvider)
        {
            _playerRequestHandlerManager = playerRequestHandlerManager;
            _playerManager = playerManager;
            _createPlayer = createPlayer;
            _connectedPlayerProvider = connectedPlayerProvider;
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
            TPlayer player = _playerManager.ConnectPlayer(Identification, _createPlayer);
            player.Connections.Add(Connection);
            return Task.CompletedTask;
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            TPlayer player = _playerManager.DisconnectPlayer(Identification);
            player.Connections.Remove(Connection);
            return Task.CompletedTask;
        }

        protected string Connection => Context.ConnectionId;
        protected TPlayer ThisPlayer => _playerManager.GetConnectedPlayerByIdentification(Identification);
        protected abstract TIdentification Identification { get; }
        protected PlayerRequestHandler RequestHandler => _playerRequestHandlerManager.GetOrCreateHandler(ThisPlayer);

        protected readonly PlayerRequestHandlerManager _playerRequestHandlerManager;
        protected readonly PlayerManager<TPlayer, TIdentification> _playerManager;
        private readonly Func<TIdentification, TPlayer> _createPlayer;
        protected readonly ConnectedPlayerProvider _connectedPlayerProvider;
    }
}
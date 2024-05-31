using backend.communication.signalR;
using backend.Data;

namespace backend.services
{
    internal class WebPlayerManager : PlayerManager<WebPlayer, PlayerIdentity>, IDisposable
    {
        public WebPlayerManager(OpponentRoboterPlayerHubManager opponentRoboterPlayerManager, OpponentRoboterPlayerHubClientManager opponentRoboterPlayerHubClientManager)
        {
            _opponentRoboterPlayerManager = opponentRoboterPlayerManager;
            _opponentRoboterPlayerHubClientManager = opponentRoboterPlayerHubClientManager;

            _opponentRoboterPlayerManager.OnPlayerConnected += OnOpponentRoboterPlayerConnected;
            _opponentRoboterPlayerManager.OnPlayerDisconnected += OnOpponentRoboterPlayerDisconnected;
            _opponentRoboterPlayerHubClientManager.OnPlayerConnected += OnOpponentRoboterPlayerConnected;
            _opponentRoboterPlayerHubClientManager.OnPlayerDisconnected += OnOpponentRoboterPlayerDisconnected;
        }

        public override WebPlayer GetConnectedPlayerByIdentification(PlayerIdentity playerIdentity)
        {
            return _connectedPlayers.First(x => x.Id == playerIdentity.Id);
        }
        public override WebPlayer GetOrCreatePlayer(PlayerIdentity playerIdentity, Func<PlayerIdentity, WebPlayer> createPlayer)
        {
            WebPlayer? player = GetConnectedPlayerOrDefault(playerIdentity.Id);
            if (player == null)
            {
                player = createPlayer(playerIdentity);
                _connectedPlayers.Add(player);
            }

            return player;
        }
        public void Dispose()
        {
            if (_disposed)
                return;

            _opponentRoboterPlayerManager.OnPlayerConnected += OnOpponentRoboterPlayerConnected;
            _opponentRoboterPlayerManager.OnPlayerDisconnected += OnOpponentRoboterPlayerDisconnected;

            _disposed = true;
        }

        private void OnOpponentRoboterPlayerConnected(OpponentRoboterPlayer opponentRoboterPlayer)
        {
            foreach (var p in _connectedPlayers)
                p.OpponentRoboterPlayerConnected(opponentRoboterPlayer);
        }
        private void OnOpponentRoboterPlayerDisconnected(OpponentRoboterPlayer opponentRoboterPlayer)
        {
            foreach (var p in _connectedPlayers)
                p.OpponentRoboterPlayerDisconnected(opponentRoboterPlayer);
        }
        private void OnOpponentRoboterPlayerConnected(OpponentRoboterPlayerHubClient opponentRoboterPlayer)
        {
            foreach (var p in _connectedPlayers)
                p.OpponentRoboterPlayerConnected(opponentRoboterPlayer);
        }
        private void OnOpponentRoboterPlayerDisconnected(OpponentRoboterPlayerHubClient opponentRoboterPlayer)
        {
            foreach (var p in _connectedPlayers)
                p.OpponentRoboterPlayerDisconnected(opponentRoboterPlayer);
        }

        private bool _disposed = false;
        private readonly OpponentRoboterPlayerHubManager _opponentRoboterPlayerManager;
        private readonly OpponentRoboterPlayerHubClientManager _opponentRoboterPlayerHubClientManager;
    }
}
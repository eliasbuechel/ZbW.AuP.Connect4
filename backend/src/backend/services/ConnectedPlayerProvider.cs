using backend.game;
using System.Diagnostics;

namespace backend.services
{
    internal class ConnectedPlayerProvider : IDisposable
    {
        public ConnectedPlayerProvider(WebPlayerManager webPlayerManager, OpponentRoboterPlayerHubManager opponentRoboterPlayerHubManager, OpponentRoboterPlayerHubClientManager opponentRoboterPlayerHubClientManager)
        {
            _webPlayerManager = webPlayerManager;
            _opponentRoboterPlayerHubManager = opponentRoboterPlayerHubManager;
            _opponentRoboterPlayerHubClientManager = opponentRoboterPlayerHubClientManager;

            _webPlayerManager.OnPlayerDisconnected += PlayerDisconnected;
            _opponentRoboterPlayerHubManager.OnPlayerDisconnected += PlayerDisconnected;
            _opponentRoboterPlayerHubClientManager.OnPlayerDisconnected += PlayerDisconnected;
        }

        public event Action<IPlayer>? OnPlayerDisconnected;

        public IEnumerable<IPlayer> WebPlayers => _webPlayerManager.ConnectedPlayers;
        public IEnumerable<IPlayer> OpponentRoboterePlayers => _opponentRoboterPlayerHubManager.ConnectedPlayers.Concat<IPlayer>(_opponentRoboterPlayerHubClientManager.ConnectedPlayers);
        public void Dispose()
        {
            if (_disposed)
            {
                Debug.Assert(false);
                return;
            }

            _disposed = true;

            _webPlayerManager.OnPlayerDisconnected -= PlayerDisconnected;
            _opponentRoboterPlayerHubManager.OnPlayerDisconnected -= PlayerDisconnected;
            _opponentRoboterPlayerHubClientManager.OnPlayerDisconnected -= PlayerDisconnected;
        }

        private void PlayerDisconnected(IPlayer player)
        {
            OnPlayerDisconnected?.Invoke(player);
        }

        public IPlayer? GetPlayer(string playerId)
        {
            foreach (var player in _webPlayerManager.ConnectedPlayers)
                if (player.Id == playerId)
                    return player;

            foreach (var player in _opponentRoboterPlayerHubManager.ConnectedPlayers)
                if (player.Id == playerId)
                    return player;

            foreach (var player in _opponentRoboterPlayerHubClientManager.ConnectedPlayers)
                if (player.Id == playerId)
                    return player;

            Debug.Assert(false);
            return null;
        }

        private bool _disposed;
        private readonly WebPlayerManager _webPlayerManager;
        private readonly OpponentRoboterPlayerHubManager _opponentRoboterPlayerHubManager;
        private readonly OpponentRoboterPlayerHubClientManager _opponentRoboterPlayerHubClientManager;
    }
}
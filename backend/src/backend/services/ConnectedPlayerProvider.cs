using backend.game;
using System.Diagnostics;

namespace backend.services
{
    internal class ConnectedPlayerProvider
    {
        public ConnectedPlayerProvider(WebPlayerManager webPlayerManager, OpponentRoboterPlayerHubManager opponentRoboterPlayerHubManager, OpponentRoboterPlayerHubClientManager opponentRoboterPlayerHubClientManager)
        {
            _webPlayerManager = webPlayerManager;
            _opponentRoboterPlayerHubManager = opponentRoboterPlayerHubManager;
            _opponentRoboterPlayerHubClientManager = opponentRoboterPlayerHubClientManager;
        }

        public IEnumerable<IPlayer> WebPlayers => _webPlayerManager.ConnectedPlayers;
        public IEnumerable<IPlayer> OpponentRoboterePlayers => _opponentRoboterPlayerHubManager.ConnectedPlayers.Concat<IPlayer>(_opponentRoboterPlayerHubClientManager.ConnectedPlayers);

        private readonly WebPlayerManager _webPlayerManager;
        private readonly OpponentRoboterPlayerHubManager _opponentRoboterPlayerHubManager;
        private readonly OpponentRoboterPlayerHubClientManager _opponentRoboterPlayerHubClientManager;

        internal IPlayer? GetPlayer(string playerId)
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
    }
}
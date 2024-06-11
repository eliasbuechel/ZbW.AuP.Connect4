using backend.communication.DOTs;
using backend.game;
using backend.Infrastructure;
using System.Diagnostics;

namespace backend.services.player
{
    internal class PlayerConnectionService : DisposingObject
    {
        public PlayerConnectionService(
            WebPlayerConnectionManager webPlayerConnectionManager,
            AlgorythmPlayerConnectionManager algorythmPlayerManager,
            OpponentRoboterPlayerConnectionManager opponentRoboterPlayerConnectionManager
            )
        {
            WebPlayerConnectionManager = webPlayerConnectionManager;
            AlgorythmPlayerConnectionManager = algorythmPlayerManager;
            OpponentRoboterPlayerConnectionManager = opponentRoboterPlayerConnectionManager;

            WebPlayerConnectionManager.OnPlayerDisconnected += PlayerDisconnected;
            OpponentRoboterPlayerConnectionManager.OnPlayerDisconnected += PlayerDisconnected;
        }

        public event Action<Player>? OnPlayerDisconnected;

        public WebPlayerConnectionManager WebPlayerConnectionManager { get; }
        public AlgorythmPlayerConnectionManager AlgorythmPlayerConnectionManager { get; }
        public OpponentRoboterPlayerConnectionManager OpponentRoboterPlayerConnectionManager { get; }

        public IEnumerable<Player> Players => WebPlayers.Concat<Player>(OpponentRoboterePlayers).Concat<Player>(AlgorythmPlayers);
        public IEnumerable<WebPlayer> WebPlayers => WebPlayerConnectionManager.ConnectedPlayers;
        public IEnumerable<OpponentRoboterPlayer> OpponentRoboterePlayers => OpponentRoboterPlayerConnectionManager.ConnectedPlayers;
        public IEnumerable<AlgorythmPlayer> AlgorythmPlayers => AlgorythmPlayerConnectionManager.ConnectedPlayers;

        public Player GetPlayer(string playerId)
        {
            foreach (var player in WebPlayerConnectionManager.ConnectedPlayers)
                if (player.Id == playerId)
                    return player;

            foreach (var player in OpponentRoboterPlayerConnectionManager.ConnectedPlayers)
                if (player.Id == playerId)
                    return player;

            foreach (var player in AlgorythmPlayerConnectionManager.ConnectedPlayers)
                if (player.Id == playerId)
                    return player;

            Debug.Assert(false);
            return null;
        }
        public ConnectedPlayersDTO GetConnectedPlayersExcept(WebPlayer requestingWebPlayer, string connectionId)
        {
            IEnumerable<ConnectedPlayerDTO> connectedWebPlayers = WebPlayers.Where(x => x.Id != requestingWebPlayer.Id).Select(x => new ConnectedPlayerDTO(x, requestingWebPlayer));
            IEnumerable<ConnectedPlayerDTO> connectedOpponentRoboterPlayers = OpponentRoboterePlayers.Where(x => x.Id != requestingWebPlayer.Id).Select(x =>
            {
                AlgorythmPlayer? algorythmPlayer = AlgorythmPlayerConnectionManager.GetConnectedPlayerByIdentificationOrDefault(x);
                if (algorythmPlayer == null)
                    return new ConnectedPlayerDTO(x);

                return new ConnectedPlayerDTO(x, algorythmPlayer);
            });

            ConnectedPlayersDTO connectedPlayers = new ConnectedPlayersDTO(connectedWebPlayers, connectedOpponentRoboterPlayers);
            return connectedPlayers;
        }

        private void PlayerDisconnected(Player player)
        {
            OnPlayerDisconnected?.Invoke(player);
        }

        protected override void OnDispose()
        {
            WebPlayerConnectionManager.OnPlayerDisconnected -= PlayerDisconnected;
            OpponentRoboterPlayerConnectionManager.OnPlayerDisconnected -= PlayerDisconnected;
        }
    }
}
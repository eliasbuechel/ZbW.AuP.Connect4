using backend.communication.DOTs;
using backend.communication.signalR.frontendApi;
using backend.Data;
using backend.game;
using Org.BouncyCastle.Asn1.Mozilla;
using System.Diagnostics;

namespace backend.services.player
{
    internal class PlayerConnectionService : IDisposable
    {
        public PlayerConnectionService(
            WebPlayerConnectionManager webPlayerConnectionManager,
            AlgorythmPlayerConnectionManager algorythmPlayerManager,
            OpponentRoboterPlayerConnectionManager opponentRoboterPlayerConnectionManager,
            FrontendApi frontendApi
            )
        {
            WebPlayerConnectionManager = webPlayerConnectionManager;
            AlgorythmPlayerConnectionManager = algorythmPlayerManager;
            OpponentRoboterPlayerConnectionManager = opponentRoboterPlayerConnectionManager;
            _frontendApi = frontendApi;

            _frontendApi.OnGetConnectedPlayers += OnGetConnectedPlayers;

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

            Debug.Assert(false);
            return null;
        }
        public WebPlayer GetWebPlayerByIdentification(string playerIdentityId)
        {
            return WebPlayerConnectionManager.GetConnectedPlayer(playerIdentityId);
        }
        public void Dispose()
        {
            if (_disposed)
            {
                Debug.Assert(false);
                return;
            }

            _disposed = true;

            _frontendApi.OnGetConnectedPlayers -= OnGetConnectedPlayers;

            WebPlayerConnectionManager.OnPlayerDisconnected -= PlayerDisconnected;
            OpponentRoboterPlayerConnectionManager.OnPlayerDisconnected -= PlayerDisconnected;
        }

        private void PlayerDisconnected(Player player)
        {
            OnPlayerDisconnected?.Invoke(player);
        }
        private async void OnGetConnectedPlayers(PlayerIdentity playerIdentity, string connectionId)
        {
            WebPlayer requester = WebPlayerConnectionManager.GetConnectedPlayer(playerIdentity);

            IEnumerable<ConnectedPlayerDTO> connectedWebPlayers = WebPlayers.Where(x => x.Id != requester.Id).Select(x => new ConnectedPlayerDTO(x, requester));
            IEnumerable<ConnectedPlayerDTO> connectedOpponentRoboterPlayers = OpponentRoboterePlayers.Where(x => x.Id != requester.Id).Select(x =>
            {
                AlgorythmPlayer? algorythmPlayer = AlgorythmPlayerConnectionManager.GetConnectedPlayer(x);
                if (algorythmPlayer == null)
                    return new ConnectedPlayerDTO(x);

                return new ConnectedPlayerDTO(x, algorythmPlayer);
            });

            ConnectedPlayersDTO connectedPlayers = new ConnectedPlayersDTO(connectedWebPlayers, connectedOpponentRoboterPlayers);
            await _frontendApi.SendConnectedPlayers(connectionId, connectedPlayers);
        }

        internal OpponentRoboterPlayer GetOponentRoboterPlayer(string opponentRoboterPlayerId)
        {
            return OpponentRoboterePlayers.Where(x => x.Id == opponentRoboterPlayerId).First();
        }

        private bool _disposed;
        private readonly FrontendApi _frontendApi;
    }
}
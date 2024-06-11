using backend.communication.DOTs;
using backend.communication.signalR.frontendApi;
using backend.Data;
using backend.game;

namespace backend.services.player
{
    internal class WebPlayerConnectionManager : PlayerConnectionManager<WebPlayer, PlayerIdentity>, IDisposable
    {
        public WebPlayerConnectionManager(
            FrontendApi frontendApi,
            OpponentRoboterPlayerConnectionManager opponentRoboterPlayerConnectionManager
            )
        {
            _frontendApi = frontendApi;
            _opponentRoboterPlayerConnectionManager = opponentRoboterPlayerConnectionManager;

            _frontendApi.OnConnected += ConnectPlayer;
            _frontendApi.OnDisconnected += DisconnectPlayer;

            _opponentRoboterPlayerConnectionManager.OnPlayerConnected += OnOpponentRoboterPlayerConnected;
            _opponentRoboterPlayerConnectionManager.OnPlayerDisconnected += OnOpponentRoboterPlayerDisconnected;
        }

        protected override void CreateOrConnectPlayer(PlayerIdentity playerIdentity, string connectionId)
        {
            WebPlayer? player = GetConnectedPlayerByIdentificationOrDefault(playerIdentity);
            if (player == null)
                player = new WebPlayer(playerIdentity);

            ConnectPlayer(player, playerIdentity, connectionId);
        }
        protected override void PlayerConnected(WebPlayer player)
        {
            base.PlayerConnected(player);

            ConnectedPlayerDTO playerDTO = new ConnectedPlayerDTO(player);
            ForeachConnectedPlayerConnectionExcept(player, async (connectionId) => await _frontendApi.PlayerConnected(connectionId, playerDTO));
        }
        protected override void PlayerDisconnected(WebPlayer player)
        {
            base.PlayerDisconnected(player);

            ForeachConnectedPlayerConnectionExcept(player, async (connectionId) => await _frontendApi.PlayerDisconnected(connectionId, player.Id));
        }
        protected override void OnDispose()
        {
            _frontendApi.OnConnected -= ConnectPlayer;
            _frontendApi.OnDisconnected -= DisconnectPlayer;

            _opponentRoboterPlayerConnectionManager.OnPlayerConnected -= OnOpponentRoboterPlayerConnected;
            _opponentRoboterPlayerConnectionManager.OnPlayerDisconnected -= OnOpponentRoboterPlayerDisconnected;
        }

        private void OnOpponentRoboterPlayerConnected(OpponentRoboterPlayer opponentRoboterPlayer)
        {
            ConnectedPlayerDTO opponentRoboterPlayerDTO = new ConnectedPlayerDTO(opponentRoboterPlayer);
            ForeachConnectedPlayerConnection(async (connectionId) => await _frontendApi.OpponentRoboterPlayerConnected(connectionId, opponentRoboterPlayerDTO));
        }
        private void OnOpponentRoboterPlayerDisconnected(OpponentRoboterPlayer opponentRoboterPlayer)
        {
            ForeachConnectedPlayerConnection(async (connectionId) => await _frontendApi.OpponentRoboterPlayerDisconnected(connectionId, opponentRoboterPlayer.Id));
        }

        private readonly FrontendApi _frontendApi;
        private readonly OpponentRoboterPlayerConnectionManager _opponentRoboterPlayerConnectionManager;
    }
}
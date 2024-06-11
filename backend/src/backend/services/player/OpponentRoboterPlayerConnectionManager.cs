using backend.communication.signalR.opponentRoboterApi;
using backend.game;
using System.ComponentModel;

namespace backend.services.player
{
    internal class OpponentRoboterPlayerConnectionManager : PlayerConnectionManager<OpponentRoboterPlayer, string>
    {
        public OpponentRoboterPlayerConnectionManager(
            OpponentRoboterHubApi opponentRoboterHubApi,
            OpponentRoboterClientApiManager opponentRoboterClientApiManager
            )
        {
            _opponentRoboterHubApi = opponentRoboterHubApi;
            _opponentRoboterClientApiManager = opponentRoboterClientApiManager;

            _opponentRoboterHubApi.OnConnected += OnConnectedOpponentRoboterHubApi;
            _opponentRoboterHubApi.OnDisconnected += DisconnectPlayer;

            _opponentRoboterClientApiManager.ForEach(x =>
            {
                x.OnConnected += OnConnectedOpponentRoboterClientApi;
                x.OnDisconnected += DisconnectPlayer;
            });

            _opponentRoboterClientApiManager.OnCreated += OnCreatedOpponentRoboterClientApi;
        }

        protected override void CreateOrConnectPlayer(string hubUrl, string connectionId)
        {
            OpponentRoboterPlayer? player = GetConnectedPlayerOrDefault(hubUrl);

            if (player == null)
                player = new OpponentRoboterPlayer(hubUrl);

            ConnectPlayer(player, hubUrl, connectionId);
        }
        protected override void OnDispose()
        {
            _opponentRoboterHubApi.OnConnected -= OnConnectedOpponentRoboterHubApi;
            _opponentRoboterHubApi.OnDisconnected -= DisconnectPlayer;

            _opponentRoboterClientApiManager.ForEach(x =>
            {
                x.OnConnected -= OnConnectedOpponentRoboterClientApi;
                x.OnDisconnected -= DisconnectPlayer;
            });

            _opponentRoboterClientApiManager.OnCreated -= OnCreatedOpponentRoboterClientApi;
        }

        private void OnCreatedOpponentRoboterClientApi(OpponentRoboterClientApi opponentRoboterClientApi)
        {
            opponentRoboterClientApi.OnConnected += OnConnectedOpponentRoboterClientApi;
            opponentRoboterClientApi.OnDisconnected += DisconnectPlayer;
        }
        private void OnConnectedOpponentRoboterClientApi(string clientUrl, string connectionId)
        {
            ConnectPlayer(clientUrl, connectionId);
            OpponentRoboterPlayer opponentRoboterPlayer = GetConnectedPlayerByIdentification(clientUrl);
            opponentRoboterPlayer.IsHubPlayer = false;
        }
        private void OnConnectedOpponentRoboterHubApi(string callerUrl, string connectionId)
        {
            ConnectPlayer(callerUrl, connectionId);
            OpponentRoboterPlayer opponentRoboterPlayer = GetConnectedPlayerByIdentification(callerUrl);
            opponentRoboterPlayer.IsHubPlayer = true;
        }

        private readonly OpponentRoboterHubApi _opponentRoboterHubApi;
        private readonly OpponentRoboterClientApiManager _opponentRoboterClientApiManager;
    }
}
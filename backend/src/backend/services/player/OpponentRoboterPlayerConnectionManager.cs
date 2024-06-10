using backend.communication.signalR.opponentRoboterApi;
using backend.game;

namespace backend.services.player
{
    internal class OpponentRoboterPlayerConnectionManager : PlayerConnectionManager<OpponentRoboterPlayer, string>
    {
        public OpponentRoboterPlayerConnectionManager(OpponentRoboterHubApi opponentRoboterHubApi)
        {
            _opponentRoboterHubApi = opponentRoboterHubApi;

            _opponentRoboterHubApi.OnConnected += ConnectOpponentRoboterHubPlayer;
            _opponentRoboterHubApi.OnDisconnected += DisconnectPlayer;
        }

        private void ConnectOpponentRoboterHubPlayer(string callerUrl, string connectionId)
        {
            ConnectPlayer(callerUrl, connectionId);
            OpponentRoboterPlayer opponentRoboterPlayer = GetConnectedPlayerByIdentification(callerUrl);
            opponentRoboterPlayer.IsHubPlayer = true;
        }

        protected override OpponentRoboterPlayer GetOrCreatePlayer(string hubUrl, string connectionId)
        {
            OpponentRoboterPlayer? player = GetConnectedPlayerOrDefault(hubUrl);

            if (player == null)
            {
                player = new OpponentRoboterPlayer(hubUrl);
                _connections.Add(new PlayerConnection(player, hubUrl, connectionId));
            }

            return player;
        }
        protected override void OnDispose()
        {
            _opponentRoboterHubApi.OnConnected -= ConnectPlayer;
            _opponentRoboterHubApi.OnDisconnected -= DisconnectPlayer;
        }

        private readonly OpponentRoboterHubApi _opponentRoboterHubApi;
    }
}
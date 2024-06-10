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

        protected override OpponentRoboterPlayer GetOrCreatePlayer(string hubUrl)
        {
            OpponentRoboterPlayer? player = GetConnectedPlayerOrDefault(hubUrl);

            if (player == null)
            {
                player = new OpponentRoboterPlayer(hubUrl);
                _connectedPlayersAndIdentification.Add(new Tuple<OpponentRoboterPlayer, string>(player, hubUrl));
            }

            return player;
        }
        protected override bool IdentificationsAreEqal(string identification1, string identification2)
        {
            return identification1 == identification2;
        }
        protected override void OnDispose()
        {
            _opponentRoboterHubApi.OnConnected -= ConnectPlayer;
            _opponentRoboterHubApi.OnDisconnected -= DisconnectPlayer;
        }

        private readonly OpponentRoboterHubApi _opponentRoboterHubApi;
    }
}
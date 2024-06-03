using backend.communication.signalR;
using backend.game;

namespace backend.services
{
    internal class OpponentRoboterPlayerHubClientManager : PlayerManager<OpponentRoboterPlayerHubClient, string, AlgorythmPlayer>
    {
        public override OpponentRoboterPlayerHubClient GetConnectedPlayerByIdentification(string hubUrl)
        {
            return _connectedPlayers.First(x => x.HubUrl == hubUrl);
        }
        protected override void PlayerConnected(OpponentRoboterPlayerHubClient player)
        {
            base.PlayerConnected(player);
            player.OnDisconnected += DisconnectRoboterPlayerHubClient;
        }

        protected override OpponentRoboterPlayerHubClient GetOrCreatePlayer(string hubUrl, Func<string, OpponentRoboterPlayerHubClient> createPlayer)
        {
            OpponentRoboterPlayerHubClient? player = _connectedPlayers.FirstOrDefault(x => x.HubUrl == hubUrl);
            if (player == null)
            {
                player = createPlayer(hubUrl);
                _connectedPlayers.Add(player);
            }

            return player;
        }
        protected override OpponentRoboterPlayerHubClient GetOrCreatePlayer(string hubUrl, AlgorythmPlayer opponentPlayer, Func<string, AlgorythmPlayer, OpponentRoboterPlayerHubClient> createPlayer)
        {
            OpponentRoboterPlayerHubClient? player = _connectedPlayers.FirstOrDefault(x => x.HubUrl == hubUrl);
            if (player == null)
            {
                player = createPlayer(hubUrl, opponentPlayer);
                _connectedPlayers.Add(player);
            }

            return player;
        }

        private void DisconnectRoboterPlayerHubClient(OpponentRoboterPlayerHubClient player)
        {
            DisconnectPlayer(player);
            player.OnDisconnected -= DisconnectRoboterPlayerHubClient;
        }
    }
}
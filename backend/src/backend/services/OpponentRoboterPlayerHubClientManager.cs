using backend.communication.signalR;

namespace backend.services
{
    internal class OpponentRoboterPlayerHubClientManager : PlayerManager<OpponentRoboterPlayerHubClient, string>
    {
        public override OpponentRoboterPlayerHubClient GetConnectedPlayerByIdentification(string hubUrl)
        {
            return _connectedPlayers.First(x => x.HubUrl == hubUrl);
        }
        public override OpponentRoboterPlayerHubClient GetOrCreatePlayer(string hubUrl, Func<string, OpponentRoboterPlayerHubClient> createPlayer)
        {
            OpponentRoboterPlayerHubClient? player = _connectedPlayers.FirstOrDefault(x => x.HubUrl == hubUrl);
            if (player == null)
            {
                player = createPlayer(hubUrl);
                _connectedPlayers.Add(player);
            }

            return player;
        }
    }
}
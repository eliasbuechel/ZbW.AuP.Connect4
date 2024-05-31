using backend.communication.signalR;
using backend.game;
using Microsoft.AspNetCore.Mvc.TagHelpers;

namespace backend.services
{
    internal class OpponentRoboterPlayerHubManager : PlayerManager<OpponentRoboterPlayer, string>
    {
        public override OpponentRoboterPlayer GetConnectedPlayerByIdentification(string connectionId)
        {
            return _connectedPlayers.First(x => x.Id == connectionId);
        }
        public override OpponentRoboterPlayer GetOrCreatePlayer(string connectionId, Func<string, OpponentRoboterPlayer> createPlayer)
        {
            OpponentRoboterPlayer? player = _connectedPlayers.FirstOrDefault(x => x.Id == connectionId);
            if (player == null)
            {
                player = createPlayer(connectionId);
                _connectedPlayers.Add(player);
            }

            return player;
        }
    }
}
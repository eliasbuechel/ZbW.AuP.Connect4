using backend.game;
using backend.services;

namespace backend.communication.DOTs
{
    internal class ConnectedPlayersDTO
    {
        public ConnectedPlayersDTO(ConnectedPlayerProvider connectedPlayerProvider, IPlayer requester)
        {
            WebPlayers = connectedPlayerProvider.WebPlayers.Where(x => x.Id != requester.Id).Select(x => new ConnectedPlayerDTO(x, requester)).ToArray();
            OpponentRoboterPlayers = connectedPlayerProvider.OpponentRoboterePlayers.Where(x => x.Id != requester.Id).Select(x => new ConnectedPlayerDTO(x, requester)).ToArray();
        }

        public ConnectedPlayerDTO[] WebPlayers { get; }
        public ConnectedPlayerDTO[] OpponentRoboterPlayers { get; }
    }
}
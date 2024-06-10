using backend.game;
using backend.services.player;

namespace backend.communication.DOTs
{
    internal class ConnectedPlayersDTO
    {
        public ConnectedPlayersDTO(IEnumerable<ConnectedPlayerDTO> webPlayers, IEnumerable<ConnectedPlayerDTO> opponentRoboterPlayers)
        {
            WebPlayers = webPlayers.ToArray();
            OpponentRoboterPlayers = opponentRoboterPlayers.ToArray();
        }

        public ConnectedPlayerDTO[] WebPlayers { get; }
        public ConnectedPlayerDTO[] OpponentRoboterPlayers { get; }
    }
}
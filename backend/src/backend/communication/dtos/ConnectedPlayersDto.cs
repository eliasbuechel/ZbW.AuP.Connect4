namespace backend.communication.dtos
{
    internal class ConnectedPlayersDto(IEnumerable<ConnectedPlayerDto> webPlayers, IEnumerable<ConnectedPlayerDto> opponentRoboterPlayers)
    {
        public ConnectedPlayerDto[] WebPlayers { get; } = webPlayers.ToArray();
        public ConnectedPlayerDto[] OpponentRoboterPlayers { get; } = opponentRoboterPlayers.ToArray();
    }
}
using backend.game.players;

namespace backend.communication.connectionManager
{
    internal class AlgorythmPlayerConnectionManager : PlayerConnectionManager<AlgorythmPlayer, Player>
    {
        protected override void CreateOrConnectPlayer(Player opponent, string connectionId)
        {
            AlgorythmPlayer? player = ConnectedPlayers.FirstOrDefault(x => x.OpponentPlayer == opponent);
            if (player == null)
                player = new AlgorythmPlayer(opponent);

            ConnectPlayer(player, opponent, connectionId);
        }
        protected override void OnDispose()
        { }
    }
}
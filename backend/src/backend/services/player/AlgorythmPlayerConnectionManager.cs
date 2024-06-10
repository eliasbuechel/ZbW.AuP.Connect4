using backend.game;

namespace backend.services.player
{
    internal class AlgorythmPlayerConnectionManager : PlayerConnectionManager<AlgorythmPlayer, Player>
    {
        protected override AlgorythmPlayer GetOrCreatePlayer(Player opponent, string connectionId)
        {
            AlgorythmPlayer? player = ConnectedPlayers.FirstOrDefault(x => x.OpponentPlayer == opponent);
            if (player == null)
            {
                player = new AlgorythmPlayer(opponent);
                _connections.Add(new PlayerConnection(player, opponent, connectionId));
            }

            return player;
        }
        protected override void OnDispose()
        { }
    }
}
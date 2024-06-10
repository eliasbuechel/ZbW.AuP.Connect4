using backend.game;

namespace backend.services.player
{
    internal class AlgorythmPlayerConnectionManager : PlayerConnectionManager<AlgorythmPlayer, Player>
    {
        protected override AlgorythmPlayer GetOrCreatePlayer(Player opponent)
        {
            AlgorythmPlayer? player = ConnectedPlayers.FirstOrDefault(x => x.OpponentPlayer == opponent);
            if (player == null)
            {
                player = new AlgorythmPlayer(opponent);
                _connectedPlayersAndIdentification.Add(new Tuple<AlgorythmPlayer, Player>(player, opponent));
            }

            return player;
        }
        protected override bool IdentificationsAreEqal(Player identification1, Player identification2)
        {
            throw new NotImplementedException();
        }
        protected override void OnDispose()
        {
        }
    }
}
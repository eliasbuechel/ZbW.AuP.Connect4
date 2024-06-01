using backend.game;

namespace backend.services
{
    internal class AlgorythmPlayerManager : PlayerManager<AlgorythmPlayer, IPlayer, IPlayer>
    {
        public override AlgorythmPlayer GetConnectedPlayerByIdentification(IPlayer opponent)
        {
            return _connectedPlayers.First(x => x.OpponentPlayer == opponent);
        }

        protected override AlgorythmPlayer GetOrCreatePlayer(IPlayer opponent, Func<IPlayer, AlgorythmPlayer> createPlayer)
        {
            AlgorythmPlayer? player = _connectedPlayers.FirstOrDefault(x => x.OpponentPlayer == opponent);
            if (player == null)
            {
                player = createPlayer(opponent);
                _connectedPlayers.Add(player);
            }

            return player;
        }
        protected override AlgorythmPlayer GetOrCreatePlayer(IPlayer identitfication, IPlayer opponent, Func<IPlayer, IPlayer, AlgorythmPlayer> createPlayer)
        {
            AlgorythmPlayer? player = _connectedPlayers.FirstOrDefault(x => x.OpponentPlayer == identitfication);
            if (player == null)
            {
                player = createPlayer(identitfication, opponent);
                _connectedPlayers.Add(player);
            }

            return player;
        }
    }
}
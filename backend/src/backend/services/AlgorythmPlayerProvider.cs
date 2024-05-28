using backend.game;
using System.Collections.Concurrent;

namespace backend.services
{
    internal class AlgorythmPlayerProvider
    {
        public IPlayer CreateAlgorythmPlayer(IPlayer requester, Func<AlgorythmPlayer> createAlgorythmPlayer)
        {
            AlgorythmPlayer algorythmPlayer = createAlgorythmPlayer();
            _requestPlayerToAlgorythmPlayerMapping.AddOrUpdate(requester, algorythmPlayer, (p, ap) => ap);
            return algorythmPlayer;
        }
        internal IPlayer GetAlgorythmPlayer(IPlayer requester)
        {
            return _requestPlayerToAlgorythmPlayerMapping[requester];
        }

        private readonly ConcurrentDictionary<IPlayer, AlgorythmPlayer> _requestPlayerToAlgorythmPlayerMapping = new ConcurrentDictionary<IPlayer, AlgorythmPlayer>();
    }
}
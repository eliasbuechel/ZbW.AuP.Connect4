using backend.game;

namespace backend.services
{
    internal class AlgorythmPlayerProvider
    {
        public AlgorythmPlayerProvider(Func<AlgorythmPlayer> createAlgorythmPlayer)
        {
            _createAlgorythmPlayer = createAlgorythmPlayer;
        }

        public IPlayer CreateAlgorythmPlayer()
        {
            return _createAlgorythmPlayer();
        }

        private readonly Func<AlgorythmPlayer> _createAlgorythmPlayer;
    }
}
namespace backend.game
{
    internal class AlgorythmPlayerProvider
    {
        public AlgorythmPlayerProvider(AlgorythmPlayer algorythmPlayer)
        {
            _algorythmPlayer = algorythmPlayer;
        }

        public IPlayer GetAlgorythmPlayer()
        {
            return _algorythmPlayer;
        }

        private readonly AlgorythmPlayer _algorythmPlayer;
    }
}

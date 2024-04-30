namespace backend.game
{
    internal class GameResult
    {
        public GameResult(IPlayer? winner, Connect4Line? line)
        {
            Winner = winner;
            Line = line;
        }
        public IPlayer? Winner { get; }
        public Connect4Line? Line { get; }
    }
}

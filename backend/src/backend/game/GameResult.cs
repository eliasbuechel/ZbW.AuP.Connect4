namespace backend.game
{
    internal class GameResult
    {
        public GameResult(IPlayer? winner, Connect4Line? line, int[] playedMoves, IPlayer startingPlayer, Match match)
        {
            Winner = winner;
            Line = line;
            PlayedMoves = playedMoves;
            StartingPlayer = startingPlayer;
            Match = match;
        }
        public IPlayer? Winner { get; }
        public Connect4Line? Line { get; }
        public int[] PlayedMoves { get; }
        public IPlayer StartingPlayer { get; }
        public Match Match { get; internal set; }
    }
}

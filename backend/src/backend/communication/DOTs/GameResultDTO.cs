using backend.game;

namespace backend.communication.DOTs
{
    internal class GameResultDTO
    {
        public GameResultDTO(GameResult gameResult)
        {
            WinnerId = gameResult.Winner?.Id;
            Line = gameResult.Line == null ? null : new Connect4LineDTO(gameResult.Line);
            PlayedMoves = gameResult.PlayedMoves;
            StartingPlayerId = gameResult.StartingPlayer.Id;
            Match = new MatchDTO(gameResult.Match);
        }

        public string? WinnerId { get; }
        public Connect4LineDTO? Line { get; }
        public int[] PlayedMoves { get; }
        public string StartingPlayerId { get; }
        public MatchDTO Match { get; }
    }
}

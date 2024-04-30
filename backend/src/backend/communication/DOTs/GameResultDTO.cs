using backend.game;

namespace backend.communication.DOTs
{
    internal class GameResultDTO
    {
        public GameResultDTO(GameResult gameResult)
        {
            WinnerId = gameResult.Winner?.Id;
            Line = gameResult.Line == null ? null : new Connect4LineDTO(gameResult.Line);
        }

        public string? WinnerId { get; }
        public Connect4LineDTO? Line { get; }
    }
}

using backend.game.entities;

namespace backend.communication.dtos
{
    internal class GameResultDto(GameResult gameResult) : EntityDto(gameResult)
    {
        public string? WinnerId { get; } = gameResult.WinnerId;
        public FieldDto[]? Line { get; } = gameResult.Line == null ? null : gameResult.Line.Select(x => new FieldDto(x)).ToArray();
        public PlayedMoveDto[] PlayedMoves { get; } = gameResult.PlayedMoves.Select(x => new PlayedMoveDto(x)).ToArray();
        public string StartingPlayerId { get; } = gameResult.StartingPlayerId;
        public GameResultMatchDto Match { get; } = new GameResultMatchDto(gameResult.Match);
        public bool HasWinnerRow { get; }
    }
}
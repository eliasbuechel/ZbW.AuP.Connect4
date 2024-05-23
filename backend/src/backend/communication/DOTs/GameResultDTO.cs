using backend.game.entities;

namespace backend.communication.DOTs
{
    internal class GameResultDTO : EntityDTO
    {
        public GameResultDTO(GameResult gameResult) : base(gameResult)
        {
            WinnerId = gameResult.WinnerId;
            Line = gameResult.Line == null ? null : gameResult.Line.Select(x => new FieldDTO(x)).ToArray();
            PlayedMoves = gameResult.PlayedMoves.ToArray();
            StartingPlayerId = gameResult.StartingPlayerId;
            Match = new GameResultMatchDTO(gameResult.Match);
        }

        public string? WinnerId { get; }
        public FieldDTO[]? Line { get; }
        public int[] PlayedMoves { get; }
        public string StartingPlayerId { get; }
        public GameResultMatchDTO Match { get; }
    }
}

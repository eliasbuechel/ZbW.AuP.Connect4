using backend.game.entities;

namespace backend.communication.DOTs
{
    internal class GameResultDTO : EntityDTO
    {
        public GameResultDTO(GameResult gameResult) : base(gameResult)
        {
            WinnerId = gameResult.WinnerId;
            Line = gameResult.Line == null ? null : gameResult.Line.Select(x => new FieldDTO(x)).ToArray(); // Proposal: change to "winningrow" 
            PlayedMoves = gameResult.PlayedMoves.Select(x => new PlayedMoveDTO(x)).ToArray();
            StartingPlayerId = gameResult.StartingPlayerId;
            Match = new GameResultMatchDTO(gameResult.Match);
        }

        public string? WinnerId { get; }
        public FieldDTO[]? Line { get; }
        public PlayedMoveDTO[] PlayedMoves { get; }
        public string StartingPlayerId { get; }
        public GameResultMatchDTO Match { get; }
        public bool HasWinnerRow { get; }
    }
}
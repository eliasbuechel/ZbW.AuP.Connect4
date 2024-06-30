using backend.game;

namespace backend.communication.dtos
{
    internal class GameDto(Game game)
    {
        public MatchDto Match { get; } = new MatchDto(game.Match);
        public string ActivePlayerId { get; } = game.ActivePlayer.Id;
        public GameBoardDto GameBoard { get; } = new GameBoardDto(game.Board);
        public FieldDto? PlacingField { get; } = game.PlacingField == null ? null : new(game.PlacingField);
        public long? MoveStartTime { get; } = game.MoveStartTime == null ? null : new DateTimeOffset(game.MoveStartTime.Value).ToUnixTimeMilliseconds();
        public long? GameStartTime { get; } = game.GameStartTime == null ? null : new DateTimeOffset(game.GameStartTime.Value).ToUnixTimeMilliseconds();
        public FieldDto? LastPlacedStone { get; } = game.LastPlacedStone == null ? null : new(game.LastPlacedStone);
        public bool IsQuittableByEveryone { get; } = game.IsQuittableByEveryone;
    }
}
using backend.game;

namespace backend.communication.DOTs
{
    internal class GameDTO(Game game)
    {
        public MatchDTO Match { get; } = new MatchDTO(game.Match);
        public string ActivePlayerId { get; } = game.ActivePlayer.Id;
        public string[][] Connect4Board { get; } = game.FieldAsIds;
        public FieldDTO? PlacingField { get; } = game.PlacingField == null ? null : new(game.PlacingField);
        public long? MoveStartTime { get; } = game.MoveStartTime == null ? null : new DateTimeOffset(game.MoveStartTime.Value).ToUnixTimeMilliseconds();
        public long? GameStartTime { get; } = game.GameStartTime == null ? null : new DateTimeOffset(game.GameStartTime.Value).ToUnixTimeMilliseconds();
        public FieldDTO? LastPlacedStone { get; } = game.LastPlacedStone == null ? null : new(game.LastPlacedStone);
        public bool IsQuittableByEveryone { get; } = game.IsQuittableByEveryone;
    }
}
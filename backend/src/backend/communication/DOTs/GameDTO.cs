using backend.game;

namespace backend.communication.DOTs
{
    internal class GameDTO(Game connect4Game)
    {
        public MatchDTO Match { get; } = new MatchDTO(connect4Game.Match);
        public string ActivePlayerId { get; } = connect4Game.ActivePlayer.Id;
        public string[][] Connect4Board { get; } = connect4Game.FieldAsIds;
        public FieldDTO? PlacingField { get; } = connect4Game.PlacingField == null ? null : new(connect4Game.PlacingField);
        public long MoveStartTime { get; } = new DateTimeOffset(connect4Game.MoveStartTime).ToUnixTimeMilliseconds();
    }
}
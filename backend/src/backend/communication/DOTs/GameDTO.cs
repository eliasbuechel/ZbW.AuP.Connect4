using backend.game;

namespace backend.communication.DOTs
{
    internal class GameDTO
    {
        public GameDTO(Game connect4Game)
        {
            Match = new MatchDTO(connect4Game.Match);
            ActivePlayerId = connect4Game.ActivePlayer.Id;
            Connect4Board = connect4Game.FieldAsIds;
            PlacingField = connect4Game.PlacingField == null ? null : new(connect4Game.PlacingField);
        }

        public MatchDTO Match { get; }
        public string ActivePlayerId { get; }
        public string[][] Connect4Board { get; }
        public FieldDTO? PlacingField { get; }
    }
}
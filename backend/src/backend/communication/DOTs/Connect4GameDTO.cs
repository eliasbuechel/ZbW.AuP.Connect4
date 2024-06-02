using backend.game;

namespace backend.communication.DOTs
{
    internal class Connect4GameDTO
    {
        public Connect4GameDTO(Connect4Game connect4Game)
        {
            Match = new MatchDTO(connect4Game.Match);
            ActivePlayerId = connect4Game.ActivePlayer.Id;
            Connect4Board = connect4Game.FieldAsIds;
            StartConfirmed = connect4Game.StartConfirmed;
            MoveStartTime = connect4Game.MoveStartTime;
        }

        public MatchDTO Match { get; }
        public string ActivePlayerId { get; }
        public string[][] Connect4Board { get; }
        public bool StartConfirmed { get; }
        public DateTime MoveStartTime { get; }
    }
}
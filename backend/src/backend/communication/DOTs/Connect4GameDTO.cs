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
            TimeSpan duration = connect4Game.MoveStartTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
            MoveStartTime = Convert.ToInt64(duration.TotalMilliseconds);
        }

        public MatchDTO Match { get; }
        public string ActivePlayerId { get; }
        public string[][] Connect4Board { get; }
        public bool StartConfirmed { get; }
        public long MoveStartTime { get; }
    }
}
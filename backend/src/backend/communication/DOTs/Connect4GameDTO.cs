using backend.game;

namespace backend.communication.DOTs
{
    internal class Connect4GameDTO
    {
        public Connect4GameDTO(Connect4Game connect4Game)
        {
            Match = new InGameMatchDTO(connect4Game.Match);
            ActivePlayerId = connect4Game.ActivePlayer.Id;
            Connect4Board = connect4Game.FieldAsIds;
            StartConfirmed = connect4Game.StartConfirmed;
        }
        public InGameMatchDTO Match { get; }
        public string ActivePlayerId { get; }
        public string[][] Connect4Board { get; }
        public bool StartConfirmed { get; }
    }
}
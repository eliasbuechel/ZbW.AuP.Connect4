using backend.game.entities;

namespace backend.communication.DOTs
{
    internal class GameResultMatchDTO
    {
        public GameResultMatchDTO(GameResultMatch match)
        {
            Player1 = new PlayerInfoDTO(match.Player1);
            Player2 = new PlayerInfoDTO(match.Player2);
        }

        public PlayerInfoDTO Player1 { get; }
        public PlayerInfoDTO Player2 { get; }
    }
}
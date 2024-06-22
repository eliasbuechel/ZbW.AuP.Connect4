using backend.game.entities;

namespace backend.communication.DOTs
{
    internal class GameResultMatchDTO(GameResultMatch match)
    {
        public PlayerInfoDTO Player1 { get; } = new PlayerInfoDTO(match.Player1);
        public PlayerInfoDTO Player2 { get; } = new PlayerInfoDTO(match.Player2);
    }
}
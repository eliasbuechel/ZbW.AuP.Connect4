using backend.game.entities;

namespace backend.communication.dtos
{
    internal class GameResultMatchDto(GameResultMatch match)
    {
        public PlayerInfoDto Player1 { get; } = new PlayerInfoDto(match.Player1);
        public PlayerInfoDto Player2 { get; } = new PlayerInfoDto(match.Player2);
    }
}
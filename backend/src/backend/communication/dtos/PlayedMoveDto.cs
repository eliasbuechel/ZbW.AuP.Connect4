using backend.game.entities;

namespace backend.communication.dtos
{
    internal class PlayedMoveDto(PlayedMove playedMove)
    {
        public int Column { get; set; } = playedMove.Column;
        public double Duration { get; set; } = playedMove.Duration.TotalMilliseconds;
    }
}
using backend.game.entities;

namespace backend.communication.DOTs
{
    internal class PlayedMoveDTO(PlayedMove playedMove)
    {
        public int Column { get; set; } = playedMove.Column;
        public double Duration { get; set; } = playedMove.Duration.TotalMilliseconds;
    }
}
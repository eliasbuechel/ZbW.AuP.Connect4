using backend.game.entities;

namespace backend.communication.DOTs
{
    internal class PlayedMoveDTO : EntityDTO
    {
        public PlayedMoveDTO(PlayedMove playedMove) : base(playedMove)
        {
            Column = playedMove.Column;
            Duration = playedMove.Duration.TotalMilliseconds;
        }

        public int Column { get; set; }
        public double Duration { get; set; }
    }
}
using backend.Data.entities;

namespace backend.game.entities
{
    internal class PlayedMove : Entity
    {
        public PlayedMove(int column, TimeSpan duration)
        {
            Column = column;
            Duration = duration;
        }
        public PlayedMove(DbPlayedMove playedMove) : base(playedMove)
        {
            Column = playedMove.Column;
            Duration = playedMove.Duration;
        }

        public int Column { get; set; }
        public TimeSpan Duration { get; set; }
    }
}

using backend.game.entities;

namespace backend.Data.entities
{
    internal class DbPlayedMove : DbEntity
    {
        public DbPlayedMove() { }
        public DbPlayedMove(PlayedMove playedMove) : base(playedMove)
        {
            Column = playedMove.Column;
            Duration = playedMove.Duration;
        }

        public int Column { get; set; }
        public TimeSpan Duration { get; set; }
    }
}

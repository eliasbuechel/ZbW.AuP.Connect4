using backend.game.entities;

namespace backend.data.entities
{
    internal class DbPlayedMove : DbEntity
    {
        public int Column { get; set; }
        public int MoveOrderIndex { get; set; }
        public TimeSpan Duration { get; set; }
    }
}

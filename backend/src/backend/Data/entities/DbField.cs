using backend.game.entities;

namespace backend.Data.entities
{
    internal class DbField : DbEntity
    {
        public int Column { get; set; }
        public int Row { get; set; }
    }
}

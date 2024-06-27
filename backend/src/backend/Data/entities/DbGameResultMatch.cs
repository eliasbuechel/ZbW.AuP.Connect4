using backend.game.entities;

namespace backend.data.entities
{
    internal class DbGameResultMatch : DbEntity
    {
        public DbGameResultMatch() { }

        public virtual DbPlayerInfo Player1 { get; set; } = new DbPlayerInfo();
        public virtual DbPlayerInfo Player2 { get; set; } = new DbPlayerInfo();
    }
}

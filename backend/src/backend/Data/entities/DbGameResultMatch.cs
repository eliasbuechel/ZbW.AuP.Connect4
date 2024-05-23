using backend.game.entities;

namespace backend.Data.entities
{
    internal class DbGameResultMatch : DbEntity
    {
        public DbGameResultMatch() { }
        public DbGameResultMatch(GameResultMatch match) : base(match)
        {
            Player1 = new DbPlayerInfo(match.Player1);
            Player2 = new DbPlayerInfo(match.Player2);
        }

        public virtual DbPlayerInfo Player1 { get; set; } = new DbPlayerInfo();
        public virtual DbPlayerInfo Player2 { get; set; } = new DbPlayerInfo();
    }
}

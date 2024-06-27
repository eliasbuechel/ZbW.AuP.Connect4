namespace backend.data.entities
{
    internal class DbGameResult : DbEntity
    {
        public string? WinnerId { get; set; } = null;
        public virtual IList<DbField> Line { get; set; } = new List<DbField>();
        public virtual IList<DbPlayedMove> PlayedMoves { get; set; } = new List<DbPlayedMove>();
        public string StartingPlayerId { get; set; } = string.Empty;
        public virtual DbGameResultMatch Match { get; set; } = new DbGameResultMatch();
    }
}

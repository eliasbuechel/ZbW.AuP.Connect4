namespace backend.data.entities
{
    internal class DbField : DbEntity
    {
        public int Column { get; set; }
        public int Row { get; set; }

        public virtual IList<DbGameResult> GameResults { get; set; } = new List<DbGameResult>();
    }
}

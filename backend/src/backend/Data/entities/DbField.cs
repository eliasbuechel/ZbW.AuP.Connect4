namespace backend.Data.entities
{
    internal class DbField : DbEntity
    {
        public int Column { get; set; }
        public int Row { get; set; }

        public virtual IList<DbGameResult> Line { get; set; } = new List<DbGameResult>();
    }
}

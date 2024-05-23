using backend.game.entities;

namespace backend.Data.entities
{
    internal class DbField : DbEntity
    {
        public DbField() { }
        public DbField(Field field) : base(field)
        {
            Column = field.Column;
            Row = field.Row;
        }

        public int Column { get; set; }
        public int Row { get; set; }
    }
}

using backend.Data.entities;

namespace backend.game.entities
{
    internal class Field : Entity
    {
        public Field(int column, int row)
        {
            Column = column;
            Row = row;
        }
        public Field(DbField field) : base(field)
        {
            Column = field.Column;
            Row = field.Row;
        }

        public int Column { get; set; }
        public int Row { get; set; }
    }
}

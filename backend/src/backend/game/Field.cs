namespace backend.game
{
    internal class Field
    {
        public Field(int column, int row)
        {
            Column = column;
            Row = row;
        }
        public int Column { get; set; }
        public int Row { get; set; }

    }
}

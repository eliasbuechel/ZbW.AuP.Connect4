namespace backend.game
{
    internal class Connect4Board
    {
        public Connect4Board()
        {
            _field = new IPlayer?[COLUMNS][];

            for (int i = 0; i < _field.Length; i++)
                _field[i] = new IPlayer?[ROWS];
        }

        public string[][] FieldAsIds
        {
            get
            {
                string[][] fieldAsIds = new string[_field.Length][];

                for (int i = 0; i < _field.Length; i++)
                {
                    fieldAsIds[i] = new string[_field[i].Length];

                    for (int j = 0; j < _field[i].Length; j++)
                    {
                        IPlayer? player = _field[i][j];
                        fieldAsIds[i][j] = player == null ? "" : player.Id;
                    }
                }

                return fieldAsIds;
            }
        }

        public bool PlaceStone(IPlayer player, int column)
        {
            if (column < 0 || column >= COLUMNS)
                return false;

            var col = _field[column];
            if (col[col.Length - 1] != null)
                return false;

            for (int i = 0; i < col.Length; i++)
            {
                if (col[i] != null)
                    continue;

                col[i] = player;
                return true;
            }

            return false;
        }

        private readonly IPlayer?[][] _field;
        private const int ROWS = 7;
        private const int COLUMNS = 6;
    }
}

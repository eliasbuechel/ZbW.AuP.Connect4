using backend.communication.mqtt;

namespace backend.game
{
    internal class Connect4Board
    {
        public Connect4Board(IRoboterAPI roboterAPI)
        {
            _field = new IPlayer?[COLUMNS][];

            for (int i = 0; i < _field.Length; i++)
                _field[i] = new IPlayer?[ROWS];
            _roboterAPI = roboterAPI;

            _roboterAPI.OnStonePlaced += OnStonePlacedOnRoboter;
            _roboterAPI.OnBoardReset += OnRoboterBoardReset;
        }
        ~Connect4Board()
        {
            _roboterAPI.OnStonePlaced -= OnStonePlacedOnRoboter;
            _roboterAPI.OnBoardReset -= OnRoboterBoardReset;
        }

        public event Action<IPlayer, Field>? OnStonePlaced;
        public event Action? OnBoardReset;

        public IPlayer?[][] Board => _field;
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
        public int Columns => COLUMNS;
        public int Rows => ROWS;
        public IPlayer?[] this[int index]
        {
            get { return _field[index]; }
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
                Field field = new Field(column, i);
                _roboterAPI.PlaceStone(player, field);
                return true;
            }

            return false;
        }
        public void Reset()
        {
            for (int i = 0; i < _field.Length; i++)
                for (int j = 0 ; j < _field[i].Length; j++)
                    _field[i][j] = null;

            _roboterAPI.ResetConnect4Board();
        }

        private void OnStonePlacedOnRoboter(IPlayer player, Field field)
        {
            OnStonePlaced?.Invoke(player, field);
        }
        private void OnRoboterBoardReset()
        {
            OnBoardReset?.Invoke();
        }

        private readonly IPlayer?[][] _field;
        private readonly IRoboterAPI _roboterAPI;
        private const int ROWS = 6;
        private const int COLUMNS = 7;
    }
}

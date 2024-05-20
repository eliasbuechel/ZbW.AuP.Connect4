using backend.communication.mqtt;

namespace backend.game
{
    internal class Connect4Board
    {
        public Connect4Board(IRoboterAPI roboterAPI)
        {
            _board = new IPlayer?[COLUMN_COUNT][];

            for (int i = 0; i < _board.Length; i++)
                _board[i] = new IPlayer?[ROW_COUNT];
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

        public IPlayer?[][] Board => _board;
        public string[][] FieldAsIds
        {
            get
            {
                string[][] fieldAsIds = new string[_board.Length][];

                for (int i = 0; i < _board.Length; i++)
                {
                    fieldAsIds[i] = new string[_board[i].Length];

                    for (int j = 0; j < _board[i].Length; j++)
                    {
                        IPlayer? player = _board[i][j];
                        fieldAsIds[i][j] = player == null ? "" : player.Id;
                    }
                }

                return fieldAsIds;
            }
        }
        public int ColumnCount => COLUMN_COUNT;
        public int RowCount => ROW_COUNT;
        public IPlayer?[] this[int index]
        {
            get { return _board[index]; }
        }

        public bool PlaceStone(IPlayer player, int column)
        {
            if (column < 0 || column >= COLUMN_COUNT)
                return false;

            var col = _board[column];
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
            for (int i = 0; i < _board.Length; i++)
                for (int j = 0 ; j < _board[i].Length; j++)
                    _board[i][j] = null;

            _roboterAPI.ResetConnect4Board();
        }

        public IPlayer?[][] GetCurrentBoardState()
        {
            IPlayer?[][] boardState = new IPlayer?[COLUMN_COUNT][];
            for (int i = 0; i < COLUMN_COUNT; i++)
            {
                boardState[i] = new IPlayer?[ROW_COUNT];
                for (int j = 0; j < ROW_COUNT; j++)
                {
                    boardState[i][j] = _board[i][j];
                }
            }

            return boardState;
        }

        private void OnStonePlacedOnRoboter(IPlayer player, Field field)
        {
            OnStonePlaced?.Invoke(player, field);
        }
        private void OnRoboterBoardReset()
        {
            OnBoardReset?.Invoke();
        }

        private readonly IPlayer?[][] _board;
        private readonly IRoboterAPI _roboterAPI;
        public const int ROW_COUNT = 6;
        public const int COLUMN_COUNT = 7;
    }
}

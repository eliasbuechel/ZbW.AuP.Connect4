
//namespace backend.game
//{
//    internal class Game
//    {
//        public Game(IPlayer player1, IPlayer player2)
//        {
//            _player1 = player1;
//            _player2 = player2;

//            _field = new SignalRPlayer?[COLUMNS][];

//            for (int i = 0; i < _field.Length; i++)
//                _field[i] = new SignalRPlayer?[ROWS];
//        }

//        public event Action<IPlayer, int>? OnMovePlayed;
//        public event Action<Connect4Line>? OnConnect4;

//        public bool GamesHasStarted { get; set; }
//        public IPlayer Player1 => _player1;
//        public IPlayer Player2 => _player2;


//        public void PlayMove(IPlayer player, int column)
//        {
//            if (_lastPlayer != null && _lastPlayer == player)
//                throw new PlayerManager("Its the other players turn.", player);

//            if (player != _player1 && player != _player2)
//                throw new PlayerManager("Player is not in the current game.", player);

//            if (column < 0 || column >= COLUMNS)
//                throw new MoveNotPossibleException("Provided column is not valid", player, column);


//            int nextFreeRow = -1;
//            for (int row = 0; row < ROWS; row++)
//            {
//                if (_field[column][row] == null)
//                {
//                    nextFreeRow = row;
//                    break;
//                }
//            }

//            if (nextFreeRow == -1)
//                throw new MoveNotPossibleException($"Column is already full!", player, column);

//            _field[column][nextFreeRow] = player;
//            _lastPlayer = player;
//            OnMovePlayed?.Invoke(player, column);

//            CheckForWin(player, column, nextFreeRow);
//        }

//        private void CheckForWin(IPlayer player, int column, int row)
//        {
//            if (CheckForWinInColumn(player, column, row))
//                return;

//            if (CheckForWinInRow(player, column, row))
//                return;

//            if (CheckForWinDiagonally(player, column, row))
//                return;
//        }
//        private bool CheckForWinInColumn(IPlayer player, int column, int row)
//        {
//            Connect4Line connect4Line = new Connect4Line();
//            int count = 4;

//            count--;
//            connect4Line[count].Column = column;
//            connect4Line[count].Row = row;

//            for (int rowDown = row - 1; rowDown >= 0; rowDown--)
//            {
//                if (_field[column][rowDown] != player)
//                    break;

//                count--;
//                connect4Line[count].Column = column;
//                connect4Line[count].Row = rowDown;

//                if (count == 0)
//                {
//                    OnConnect4?.Invoke(connect4Line);
//                    return true;
//                }
//            }

//            return false;
//        }
//        private bool CheckForWinInRow(IPlayer player, int column, int row)
//        {
//            Connect4Line connect4Line = new Connect4Line();
//            int count = 0;

//            for (int i = 0; i < COLUMNS; i++)
//            {
//                if (_field[i][row] == player)
//                {
//                    connect4Line[count].Column = i;
//                    connect4Line[count].Row = row;
//                    count++;
//                }
//                else
//                    count = 0;

//                if (count == 4)
//                {
//                    OnConnect4?.Invoke(connect4Line);
//                    return true;
//                }
//            }

//            return false;
//        }
//        private bool CheckForWinDiagonally(IPlayer player, int column, int row)
//        {
//            Connect4Line connect4Line = new Connect4Line();
//            int c = column;
//            int r = row;

//            while(true)
//            {
//                if (c <= 0 || r <= 0)
//                    break;

//                c--;
//                r--;
//            }

//            int count = 0;
//            while(c < COLUMNS && r < ROWS)
//            {
//                if (_field[c][r] == player)
//                {
//                    connect4Line[count].Column = c;
//                    connect4Line[count].Row = r;
//                    count++;
//                }
//                else
//                    count = 0;

//                if (count == 4)
//                {
//                    OnConnect4?.Invoke(connect4Line);
//                    return true;
//                }

//                c++;
//                r++;
//            }

//            return false;
//        }

//        private IPlayer? _lastPlayer;
//        private readonly IPlayer _player1;
//        private readonly IPlayer _player2;
//        private IPlayer?[][] _field;
//        private const int ROWS = 7;
//        private const int COLUMNS = 6;
//    }
//}

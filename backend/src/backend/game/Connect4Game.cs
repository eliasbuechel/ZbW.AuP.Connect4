using System.Diagnostics;

namespace backend.game
{
    internal class Connect4Game : IDisposable
    {
        public Connect4Game(Match match, Connect4Board connect4Board)
        {
            _match = match;
            _connect4Board = connect4Board;

            _connect4Board.OnStonePlaced += OnStonePlaced;
            _connect4Board.OnBoardReset += OnBoardReset;

            _startingPlayer = match.Player1;
            _activePlayer = _startingPlayer;
        }

        public event Action? OnGameEnded;

        public Match Match => _match;
        public IPlayer ActivePlayer => _activePlayer;
        public string[][] FieldAsIds => _connect4Board.FieldAsIds;
        public bool StartConfirmed => _match.Player1.HasConfirmedGameStart && _match.Player2.HasConfirmedGameStart;

        public void PlayMove(IPlayer player, int column)
        {
            if (_activePlayer != player)
            {
                Debug.Assert(false);
                return;
            }
            
            if (!_connect4Board.PlaceStone(player, column))
            {
                Debug.Assert(false);
                return;
            }
            _playedMoves.Add(column);
        }
        public void PlayerQuit(IPlayer player)
        {
            IPlayer winner = player == _match.Player1 ? _match.Player2 : _match.Player1;
            GameResult gameResult = new GameResult(winner, null, _playedMoves.ToArray(), _startingPlayer, _match);
            GameEndet(gameResult);
        }
        public void Initialize()
        {
            _connect4Board.Reset();
        }
        public void ConnfirmedGameStart(IPlayer player)
        {
            IPlayer opponent = _match.Player1 == player ? _match.Player2 : _match.Player1;
            player.YouConfirmedGameStart();
            opponent.OpponentConfirmedGameStart();
            if (opponent.HasConfirmedGameStart)
            {
                _match.Player1.GameStartConfirmed();
                _match.Player2.GameStartConfirmed();
            }
        }
        public int GetBestMove()
        {
            IPlayer?[][] board = GetBoardDeepCopy(_connect4Board.Board);

            IPlayer maxPlayer = _match.Player1 == _activePlayer ? _match.Player1 : _match.Player2;
            IPlayer minPlayer = _match.Player1 == _activePlayer ? _match.Player2 : _match.Player1;

            int value = int.MinValue;
            int bestColumn = 0;

            for (int i = 0; i < board.Length; i++)
            {
                for (int j = 0; j < board[i].Length; j++)
                {
                    if (board[i][j] != null)
                        continue;

                    board[i][j] = _activePlayer;

                    int miniMaxValue = MiniMax(board, 1, maxPlayer, minPlayer, _activePlayer, i, j);
                    if (miniMaxValue > value)
                    {
                        value = miniMaxValue;
                        bestColumn = i;
                    }

                    board[i][j] = null;
                    break;
                }
            }

            return bestColumn;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                Debug.Assert(false);
                return;
            }

            _connect4Board.OnStonePlaced -= OnStonePlaced;
            _connect4Board.OnBoardReset -= OnBoardReset;
            _disposed = true;
        }

        private IPlayer?[][] GetBoardDeepCopy(IPlayer?[][] field)
        {
            IPlayer?[][] newField = new IPlayer[field.Length][];
            for (int i = 0; i < newField.Length; i++)
            {
                newField[i] = new IPlayer?[field[i].Length];
                for (int j = 0; j < newField[i].Length; j++)
                {
                    newField[i][j] = field[i][j];
                }
            }
            return newField;
        }

        private void OnBoardReset()
        {
            _match.Player1.GameStarted(this);
            _match.Player2.GameStarted(this);
        }
        private void OnStonePlaced(IPlayer player, Field field)
        {
            SwapActivePlayer();
            CheckForWin(field, player);

            Match.Player1.MovePlayed(player, field);
            Match.Player2.MovePlayed(player, field);
        }
        private void SwapActivePlayer()
        {
            _activePlayer = _activePlayer == _match.Player1 ? _match.Player2 : _match.Player1;
        }
        private void CheckForWin(Field field, IPlayer player)
        {
            if (CheckForWinInColumn(field, player))
                return;
            if (CheckForWinInRow(field, player))
                return;
            if (CheckForWinDiagonallyUp(field, player))
                return;
            if (CheckForWinDiagonallyDown(field, player))
                return;
            if (CheckForNoMoveLeft())
                return;
        }
        private bool CheckForWinInColumn(Field lastPlacedStone, IPlayer player)
        {
            Connect4Line connect4Line = new Connect4Line();

            int count = 4;
            count--;
            connect4Line[count].Column = lastPlacedStone.Column;
            connect4Line[count].Row = lastPlacedStone.Row;

            for (int rowDown = lastPlacedStone.Row - 1; rowDown >= 0; rowDown--)
            {
                if (_connect4Board[lastPlacedStone.Column][rowDown] != player)
                    break;

                Field field = new Field(lastPlacedStone.Column, rowDown);
                count--;
                connect4Line[count].Column = lastPlacedStone.Column;
                connect4Line[count].Row = rowDown;

                if (count == 0)
                {
                    OnConnect4(connect4Line);
                    return true;
                }
            }

            return false;
        }
        private bool CheckForWinInRow(Field lastPlacedStone, IPlayer player)
        {
            Connect4Line connect4Line = new Connect4Line();
            int count = 0;

            for (int i = 0; i < _connect4Board.Columns; i++)
            {
                if (_connect4Board[i][lastPlacedStone.Row] == player)
                {
                    connect4Line[count].Column = i;
                    connect4Line[count].Row = lastPlacedStone.Row;
                    count++;
                }
                else
                    count = 0;

                if (count == 4)
                {
                    OnConnect4(connect4Line);
                    return true;
                }
            }

            return false;
        }
        private bool CheckForWinDiagonallyUp(Field lastPlacedStone, IPlayer player)
        {
            Connect4Line connect4Line = new Connect4Line();
            int c = lastPlacedStone.Column;
            int r = lastPlacedStone.Row;

            while (true)
            {
                if (c <= 0 || r <= 0)
                    break;

                c--;
                r--;
            }

            int count = 0;
            while (c < _connect4Board.Columns && r < _connect4Board.Rows)
            {
                if (_connect4Board[c][r] == player)
                {
                    connect4Line[count].Column = c;
                    connect4Line[count].Row = r;
                    count++;
                }
                else
                    count = 0;

                if (count == 4)
                {
                    OnConnect4(connect4Line);
                    return true;
                }

                c++;
                r++;
            }

            return false;
        }
        private bool CheckForWinDiagonallyDown(Field lastPlacedStone, IPlayer player)
        {
            Connect4Line connect4Line = new Connect4Line();
            int c = lastPlacedStone.Column;
            int r = lastPlacedStone.Row;

            while (true)
            {
                if (c <= 0 || r >= _connect4Board.Rows - 1)
                    break;

                c--;
                r++;
            }

            int count = 0;
            while (c < _connect4Board.Columns && r >= 0)
            {
                if (_connect4Board[c][r] == player)
                {
                    connect4Line[count].Column = c;
                    connect4Line[count].Row = r;
                    count++;
                }
                else
                    count = 0;

                if (count == 4)
                {
                    OnConnect4(connect4Line);
                    return true;
                }

                c++;
                r--;
            }

            return false;
        }
        private bool CheckForNoMoveLeft()
        {
            if (!HasNoMoveLeft(_connect4Board.Board))
                return false;

            OnNoMoveLeft();
            return true;
        }
        private void OnConnect4(Connect4Line connect4Line)
        {
            GameResult gameResult = new GameResult(_activePlayer, connect4Line, _playedMoves.ToArray(), _startingPlayer, _match);
            GameEndet(gameResult);
        }
        private void OnNoMoveLeft()
        {
            GameResult gameResult = new GameResult(null, null, _playedMoves.ToArray(), _startingPlayer, _match);
            GameEndet(gameResult);
        }
        private void GameEndet(GameResult gameResult)
        {
            _match.Player1.GameEnded(gameResult);
            _match.Player2.GameEnded(gameResult);
            _match.Player1.HasConfirmedGameStart = false;
            _match.Player2.HasConfirmedGameStart = false;
            OnGameEnded?.Invoke();
        }


        private int MiniMax(IPlayer?[][] board, int depth, IPlayer maxPlayer, IPlayer minPlayer, IPlayer activePlayer, int colLastMove, int rowLastMove)
        {
            const int MAX_DEPTH = 6;

            if (depth >= MAX_DEPTH) return 0;

            IPlayer lastMovePlayer = maxPlayer == activePlayer ? minPlayer : maxPlayer;
            if (HasNoMoveLeft(board))
                return 0;
            else if (HasWon(board, lastMovePlayer, colLastMove, rowLastMove))
            {
                if (lastMovePlayer == maxPlayer)
                    return MAX_DEPTH - depth;

                return -MAX_DEPTH + depth;
            }


            if (maxPlayer == activePlayer)
            {
                int value = int.MinValue;

                for (int i = 0; i < board.Length; i++)
                {
                    for (int j = 0; j < board[i].Length; j++)
                    {
                        if (board[i][j] != null)
                            continue;

                        board[i][j] = activePlayer;
                        Max(value, MiniMax(board, depth + 1, maxPlayer, minPlayer, minPlayer, i, j));
                        board[i][j] = null;
                    }
                }

                return value;
            }
            else
            {
                int value = int.MaxValue;

                for (int i = 0; i < board.Length; i++)
                {
                    for (int j = 0; j < board[i].Length; j++)
                    {
                        if (board[i][j] != null)
                            continue;

                        board[i][j] = activePlayer;
                        Min(value, MiniMax(board, depth + 1, maxPlayer, minPlayer, maxPlayer, i, j));
                        board[i][j] = null;
                    }
                }

                return value;
            }
        }

        private bool HasWon(IPlayer?[][] board, IPlayer lastMovePlayer, int col, int row)
        {
            // column
            int count = 1;
            int i = row - 1;
            while (i >= 0)
            {
                if (board[col][i] != lastMovePlayer)
                    break;

                count++;
                i--;
            }

            if (count >= 4)
                return true;

            // row
            count = 1;
            i = col - 1;
            while(i >= 0)
            {
                if (board[i][row] != lastMovePlayer)
                    break;

                count++;
                i--;
            }

            i = col + 1;
            while (i < board.Length)
            {
                if (board[i][row] != lastMovePlayer)
                    break;

                count++;
                i++;
            }

            if (count >= 4)
                return true;

            // diagonally up
            count = 1;
            i = col - 1;
            int j = row - 1;

            while (i >= 0 && j >= 0)
            {
                if (board[i][j] != lastMovePlayer)
                    break;

                count++;
                i--;
                j--;
            }

            i = col + 1;
            j = row + 1;

            while (i < board.Length && j < board[i].Length)
            {
                if (board[i][j] != lastMovePlayer)
                    break;

                count++;
                i++;
                j++;
            }

            if (count >= 4)
                return true;

            // diagonally down

            count = 1;
            i = col - 1;
            j = row + 1;

            while (i >= 0 && j < board[i].Length)
            {
                if (board[i][j] != lastMovePlayer)
                    break;

                count++;
                i--;
                j++;
            }

            i = col + 1;
            j = row - 1;

            while (i < board.Length && j >= 0)
            {
                if (board[i][j] != lastMovePlayer)
                    break;

                count++;
                i++;
                j--;
            }

            if (count >= 4)
                return true;


            return false;
        }

        private bool HasNoMoveLeft(IPlayer?[][] board)
        {
            for (int i = 0; i < board.Length; i++)
                if (board[i][board[i].Length - 1] == null)
                    return false;

            return true;
        }

        private int Max(int a, int b)
        {
            if (a > b)
                return a;

            return b;
        }

        private int Min(int a, int b)
        {
            if (a < b)
                return a;

            return b;
        }

        private bool _disposed = false;
        private IPlayer _activePlayer;
        private readonly IPlayer _startingPlayer;
        private readonly Match _match;
        private readonly Connect4Board _connect4Board;
        private readonly ICollection<int> _playedMoves = new List<int>();
    }
}

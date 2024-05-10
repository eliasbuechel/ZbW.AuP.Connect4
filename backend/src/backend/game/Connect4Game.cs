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

        public Guid Id { get; } = new Guid();
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

        private void OnBoardReset()
        {
            _match.Player1.GameStarted(this);
            _match.Player2.GameStarted(this);
        }
        private void OnStonePlaced(IPlayer player, Field field)
        {
            Match.Player1.MovePlayed(player, field);
            Match.Player2.MovePlayed(player, field);

            CheckForWin(field);
            SwapActivePlayer();
        }
        private void SwapActivePlayer()
        {
            _activePlayer = _activePlayer == _match.Player1 ? _match.Player2 : _match.Player1;
        }
        private void CheckForWin(Field field)
        {
            if (CheckForWinInColumn(field))
                return;
            if (CheckForWinInRow(field))
                return;
            if (CheckForWinDiagonallyUp(field))
                return;
            if (CheckForWinDiagonallyDown(field))
                return;
            if (CheckForNoMoveLeft())
                return;
        }
        private bool CheckForWinInColumn(Field lastPlacedStone)
        {
            Connect4Line connect4Line = new Connect4Line();

            int count = 4;
            count--;
            connect4Line[count].Column = lastPlacedStone.Column;
            connect4Line[count].Row = lastPlacedStone.Row;

            for (int rowDown = lastPlacedStone.Row - 1; rowDown >= 0; rowDown--)
            {
                if (_connect4Board[lastPlacedStone.Column][rowDown] != _activePlayer)
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
        private bool CheckForWinInRow(Field lastPlacedStone)
        {
            Connect4Line connect4Line = new Connect4Line();
            int count = 0;

            for (int i = 0; i < _connect4Board.Columns; i++)
            {
                if (_connect4Board[i][lastPlacedStone.Row] == _activePlayer)
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
        private bool CheckForWinDiagonallyUp(Field lastPlacedStone)
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
                if (_connect4Board[c][r] == _activePlayer)
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
        private bool CheckForWinDiagonallyDown(Field lastPlacedStone)
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
                if (_connect4Board[c][r] == _activePlayer)
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
            bool allCollumnsFull = true;
            for (int i = 0; i  < _connect4Board.Columns; i++)
            {
                IPlayer?[] column = _connect4Board[i];

                if (column[_connect4Board[i].Length - 1] == null)
                    allCollumnsFull = false;
            }

            if (!allCollumnsFull)
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

        private bool _disposed = false;
        private IPlayer _activePlayer;
        private readonly IPlayer _startingPlayer;
        private readonly Match _match;
        private readonly Connect4Board _connect4Board;
        private readonly ICollection<int> _playedMoves = new List<int>();
    }
}

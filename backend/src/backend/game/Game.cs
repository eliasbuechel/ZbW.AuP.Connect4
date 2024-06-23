using backend.game.entities;
using backend.utilities;
using System.Diagnostics;

namespace backend.game
{
    internal class Game : IDisposable
    {
        public Game(Match match, GameBoard connect4Board)
        {
            _match = match;
            _gameBoard = connect4Board;

            _gameBoard.OnStonePlaced += OnStonePlaced;
            _gameBoard.OnBoardReset += OnBoardReset;


            if (match.Player1 is WebPlayer || match.Player2 is WebPlayer)
            {
                Random random = new();
                _startingPlayer = random.Next(0, 2) == 0 ? match.Player1 : match.Player2;
            }
            else
                _startingPlayer = match.Player2;

            _activePlayer = _startingPlayer;

            _match.Player1.IsInGame = true;
            _match.Player2.IsInGame = true;
        }

        public event Action<GameResult>? OnGameEnded;
        public event Action<Player, Field>? OnMovePlayed;
        public event Action<Game>? OnGameStarted;

        public Guid Id { get; } = new Guid();
        public Match Match => _match;
        public Player ActivePlayer => _activePlayer;
        public string[][] FieldAsIds => _gameBoard.FieldAsIds;
        public bool GameEnded => _gameEnded;
        public DateTime MoveStartTime => _moveStartingTime;


        public Field? PlacingField => _gameBoard.PlacingField;
        public Field? LastPlacedStone { get; private set; }

        public void PlayMove(Player player, int column)
        {
            if (_activePlayer != player)
                throw new InvalidPlayerRequestException($"Play move exception [player:{player.Username} column:{column}]. Not the players turn!");

            if (_activePlayerPlacedStone)
                throw new InvalidPlayerRequestException($"Play move exception [player:{player.Username} column:{column}]. Already played move in his turn!");

            if (column < 0 || column > GameBoard.Columns)
                throw new InvalidPlayerRequestException($"Play move exception [player:{player.Username} column:{column}]. Column is not in valid range");

            _gameBoard.PlaceStone(player, column);

            TimeSpan duration = DateTime.Now - _moveStartingTime;
            PlayedMove playedMove = new(column, duration);
            _activePlayer.TotalPlayTime += duration;
            _playedMoves.Add(playedMove);
            _moveStartingTime = DateTime.Now;
            _activePlayerPlacedStone = true;
        }
        public void PlayerQuit(Player player)
        {
            if (!_match.Player1.Equals(player) && !_match.Player2.Equals(player))
                throw new InvalidPlayerRequestException($"Quit game exception [player:{player.Username}]. Quitting player is not part of the active game.");

            Player winner = player == _match.Player1 ? _match.Player2 : _match.Player1;
            bool hasWinnerRow = false;
            GameResult gameResult = new GameResult(winner, null, _playedMoves.ToArray(), _startingPlayer, _match, hasWinnerRow);
            OnGameEndet(gameResult);
        }
        public void Initialize()
        {
            _gameBoard.Reset();
        }
        public int GetBestMove(Player player)
        {
            const int LOOK_AHEAD_MOVES = 10;
            const int INVALID_BEST_MOVE = -1;

            Player opponent = _match.Player1 == player ? _match.Player2 : _match.Player1;

            int value = int.MinValue;
            int bestMove = INVALID_BEST_MOVE;
            int alpha = int.MinValue;
            int beta = int.MaxValue;

            foreach (int col in _columnOrder)
            {
                if (!GetNextFreeRow(col, out int row))
                    continue;

                Debug.Assert(row >= 0);

                _gameBoard[col][row] = player;

                int miniMaxValue;

                if (NoMoveLeft())
                    miniMaxValue = 0;
                else if (HasWon(player, col, row))
                    miniMaxValue = LOOK_AHEAD_MOVES * 1000;
                else if (LOOK_AHEAD_MOVES <= 1)
                    miniMaxValue = CalculateBoardValue(player);
                else
                    miniMaxValue = MiniMax(LOOK_AHEAD_MOVES - 1, player, opponent, false, alpha, beta);

                _gameBoard[col][row] = null;

                if (miniMaxValue > value)
                {
                    bestMove = col;
                    value = miniMaxValue;
                }

                alpha = Math.Max(alpha, value);
                if (alpha >= beta)
                    break;
            }

            return bestMove;
        }
        public void PlayManualMove(int column)
        {
            if (_gameEnded) return;
            PlayMove(_activePlayer, column);
        }
        public void Dispose()
        {
            if (_disposed)
            {
                Debug.Assert(false);
                return;
            }

            _disposed = true;

            _gameBoard.OnStonePlaced -= OnStonePlaced;
            _gameBoard.OnBoardReset -= OnBoardReset;
        }

        private void OnBoardReset()
        {
            OnGameStarted?.Invoke(this);
        }
        private void OnStonePlaced(Player player, Field field)
        {
            SwapActivePlayer();
            LastPlacedStone = field;
            OnMovePlayed?.Invoke(player, field);
            CheckForWin(field, player);
        }
        private void SwapActivePlayer()
        {
            _activePlayer = _activePlayer == _match.Player1 ? _match.Player2 : _match.Player1;
            _activePlayerPlacedStone = false;
        }
        private void CheckForWin(Field field, Player player)
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
        private bool CheckForWinInColumn(Field lastPlacedStone, Player player)
        {
            Field[] line = new Field[4];
            int count = 0;

            line[count++] = new Field(lastPlacedStone.Column, lastPlacedStone.Row);

            for (int rowDown = lastPlacedStone.Row - 1; rowDown >= 0; rowDown--)
            {
                if (_gameBoard[lastPlacedStone.Column][rowDown] != player)
                    break;

                line[count++] = new Field(lastPlacedStone.Column, rowDown);

                if (count >= 4)
                {
                    OnConnect4(line, player);
                    return true;
                }
            }

            return false;
        }
        private bool CheckForWinInRow(Field lastPlacedStone, Player player)
        {
            Field[] line = new Field[4];
            int count = 0;

            line[count++] = new Field(lastPlacedStone.Column, lastPlacedStone.Row);

            int col = lastPlacedStone.Column - 1;
            while (col >= 0 && count < 4)
            {
                if (_gameBoard[col][lastPlacedStone.Row] != player)
                    break;

                line[count++] = new Field(col, lastPlacedStone.Row);
                col--;
            }

            col = lastPlacedStone.Column + 1;
            while (col < GameBoard.Columns && count < 4)
            {
                if (_gameBoard[col][lastPlacedStone.Row] != player)
                    break;

                line[count++] = new Field(col, lastPlacedStone.Row);
                col++;
            }

            if (count >= 4)
            {
                OnConnect4(line, player);
                return true;
            }

            return false;
        }
        private bool CheckForWinDiagonallyUp(Field lastPlacedStone, Player player)
        {
            Field[] line = new Field[4];
            int count = 0;

            line[count++] = new Field(lastPlacedStone.Column, lastPlacedStone.Row);

            int col = lastPlacedStone.Column - 1;
            int row = lastPlacedStone.Row - 1;
            while (col >= 0 && row >= 0 && count < 4)
            {
                if (_gameBoard[col][row] != player)
                    break;
                
                line[count++] = new Field(col, row);
                col--;
                row--;
            }

            col = lastPlacedStone.Column + 1;
            row = lastPlacedStone.Row + 1;
            while (col < GameBoard.Columns && row < GameBoard.Rows && count < 4)
            {
                if (_gameBoard[col][row] != player)
                    break;
               
                line[count++] = new Field(col, row);
                col++;
                row++;
            }

            if (count >= 4)
            {
                OnConnect4(line, player);
                return true;
            }

            return false;
        }
        private bool CheckForWinDiagonallyDown(Field lastPlacedStone, Player player)
        {
            Field[] line = new Field[4];
            int count = 0;

            line[count++] = new Field(lastPlacedStone.Column, lastPlacedStone.Row);

            int col = lastPlacedStone.Column - 1;
            int row = lastPlacedStone.Row + 1;
            while (col >= 0 && row < GameBoard.Rows && count < 4)
            {
                if (_gameBoard[col][row] != player)
                    break;
                
                line[count++] = new Field(col, row);
                col--;
                row++;
            }

            col = lastPlacedStone.Column + 1;
            row = lastPlacedStone.Row - 1;
            while (col < GameBoard.Columns && row >= 0 && count < 4)
            {
                if (_gameBoard[col][row] != player)
                    break;

                line[count++] = new Field(col, row);
                col++;
                row--;
            }

            if (count >= 4)
            {
                OnConnect4(line, player);
                return true;
            }

            return false;
        }
        private bool CheckForNoMoveLeft()
        {
            bool allCollumnsFull = true;
            for (int i = 0; i  < GameBoard.Columns; i++)
            {
                Player?[] column = _gameBoard[i];

                if (column[_gameBoard[i].Length - 1] == null)
                    allCollumnsFull = false;
            }

            if (!allCollumnsFull)
                return false;

            OnNoMoveLeft();
            return true;
        }
        private void OnConnect4(ICollection<Field> connect4Line, Player player)
        {
            bool hasWinnerRow = true;
            GameResult gameResult = new GameResult(player, connect4Line, _playedMoves.ToArray(), _startingPlayer, _match, hasWinnerRow);
            OnGameEndet(gameResult);
        }
        private void OnNoMoveLeft()
        {
            bool hasWinnerRow = false;
            GameResult gameResult = new GameResult(null, null, _playedMoves.ToArray(), _startingPlayer, _match, hasWinnerRow);
            OnGameEndet(gameResult);
        }
        private void OnGameEndet(GameResult gameResult)
        {
            _match.Player1.HasConfirmedGameStart = false;
            _match.Player2.HasConfirmedGameStart = false;

            _gameEnded = true;
            OnGameEnded?.Invoke(gameResult);
        }

        private int MiniMax(int depth, Player maxPlayer, Player minPlayer, bool maximizing, int alpha, int beta)
        {
            int value;

            if (maximizing)
            {
                value = int.MinValue;


                foreach (int col in _columnOrder)
                {
                    if (!GetNextFreeRow(col, out int row))
                        continue;

                    Debug.Assert(row >= 0);

                    _gameBoard[col][row] = maxPlayer;

                    int miniMaxValue;

                    if (NoMoveLeft())
                        miniMaxValue = 0;
                    else if (HasWon(maxPlayer, col, row))
                        miniMaxValue = depth * 1000;
                    else if (depth <= 1)
                        miniMaxValue = CalculateBoardValue(maxPlayer);
                    else
                        miniMaxValue = MiniMax(depth - 1, maxPlayer, minPlayer, false, alpha, beta);

                    _gameBoard[col][row] = null;

                    value = Math.Max(value, miniMaxValue);
                    alpha = Math.Max(alpha, value);
                    if (alpha >= beta)
                        break;
                }
            }
            else
            {
                value = int.MaxValue;

                foreach (int col in _columnOrder)
                {
                    if (!GetNextFreeRow(col, out int row))
                        continue;

                    Debug.Assert(row >= 0);

                    _gameBoard[col][row] = minPlayer;

                    int miniMaxValue;

                    if (NoMoveLeft())
                        miniMaxValue = 0;
                    else if (HasWon(minPlayer, col, row))
                        miniMaxValue = depth * -1000;
                    else if (depth <= 1)
                        miniMaxValue = CalculateBoardValue(maxPlayer);
                    else
                        miniMaxValue = MiniMax(depth - 1, maxPlayer, minPlayer, true, alpha, beta);

                    _gameBoard[col][row] = null;

                    value = Math.Min(value, miniMaxValue);
                    beta = Math.Min(beta, value);
                    if (alpha >= beta)
                        break;
                }
            }

            return value;
        }
        private bool GetNextFreeRow(int col, out int row)
        {
            int j = 0;
            while (j < GameBoard.Rows)
            {
                if (_gameBoard[col][j] == null)
                {
                    row = j;
                    return true;
                }

                j++;
            }

            row = -1;
            return false;
        }
        private int CalculateBoardValue(Player maxPlayer)
        {
            int boardValue = 0;

            for (int i = 0; i < GameBoard.Columns; i++)
            {
                for (int j = 0; j < GameBoard.Rows; j++)
                {
                    if (_gameBoard[i][j] == maxPlayer)
                        boardValue += _propabilityMatrix[i][j];
                    else if (_gameBoard[i][j] != null)
                        boardValue -= _propabilityMatrix[i][j];
                    else
                        break;
                }
            }

            return boardValue;
        }
        private bool NoMoveLeft()
        {
            for (int i = 0; i < GameBoard.Columns; i++)
                if (_gameBoard[i][GameBoard.Rows - 1] == null)
                    return false;

            return true;
        }
        private bool HasWon(Player player, int col, int row)
        {
            // vertically

            int count = 1;
            int j = row - 1;
            while (j >= 0)
            {
                if (_gameBoard[col][j] != player)
                    break;

                j--;
                count++;
            }
            if (count >= 4)
                return true;

            // horizontally
            count = 1;
            int i = col - 1;
            while (i >= 0)
            {
                if (_gameBoard[i][row] != player)
                    break;

                i--;
                count++;
            }
            i = col + 1;
            while(i < GameBoard.Columns)
            {
                if (_gameBoard[i][row] != player)
                    break;

                i++;
                count++;
            }
            if (count >= 4)
                return true;

            // diagonally up
            count = 1;
            i = col - 1;
            j = row - 1;
            while (i >= 0 && j >= 0)
            {
                if (_gameBoard[i][j] != player)
                    break;

                i--;
                j--;
                count++;
            }
            i = col + 1;
            j = row + 1;
            while (i < GameBoard.Columns && j < GameBoard.Rows)
            {
                if (_gameBoard[i][j] != player)
                    break;

                i++;
                j++;
                count++;
            }
            if (count >= 4)
                return true;

            // diagonally down
            count = 1;
            i = col - 1;
            j = row + 1;
            while (i >= 0 && j < GameBoard.Rows)
            {
                if (_gameBoard[i][j] != player)
                    break;

                i--;
                j++;
                count++;
            }
            i = col + 1;
            j = row - 1;
            while (i < GameBoard.Columns && j >= 0)
            {
                if (_gameBoard[i][j] != player)
                    break;

                i++;
                j--;
                count++;
            }
            if (count >= 4)
                return true;

            return false;
        }

        private DateTime _moveStartingTime = DateTime.Now;
        private bool _gameEnded;
        private bool _disposed = false;
        private Player _activePlayer;
        private bool _activePlayerPlacedStone;
        private readonly Player _startingPlayer;
        private readonly Match _match;
        private readonly GameBoard _gameBoard;
        private readonly List<PlayedMove> _playedMoves = [];
        private readonly int[] _columnOrder = [3, 2, 4, 1, 5, 0, 6];
        private readonly int[][] _propabilityMatrix = [[3, 4, 5, 5, 4, 3],
                                                       [4, 6, 8, 8, 6, 4],
                                                       [5, 8, 11, 11, 8, 5],
                                                       [7, 10, 13, 13, 10, 7],
                                                       [5, 8, 11, 11, 8, 5],
                                                       [4, 6, 8, 8, 6, 4],
                                                       [3, 4, 5, 5, 4, 3]];
    }
}

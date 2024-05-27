﻿using backend.game.entities;
using backend.services;
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
            _gameTimeService = new GameTimeService();
        }

        public event Action<GameResult>? OnGameEnded;

        public Guid Id { get; } = new Guid();
        public Match Match => _match;
        public IPlayer ActivePlayer => _activePlayer;
        public string[][] FieldAsIds => _connect4Board.FieldAsIds;
        public bool StartConfirmed => _match.Player1.HasConfirmedGameStart && _match.Player2.HasConfirmedGameStart;

        public void PlayMove(IPlayer player, int column)
        {
            if (_activePlayer != player)
            {
                return;
            }
            
            if (!_connect4Board.PlaceStone(player, column))
            {
                Debug.Assert(false);
                return;
            }
            TimeSpan duration = DateTime.Now - _moveStartingTime;
            PlayedMove playedMove = new PlayedMove(column, duration);
            _playedMoves.Add(playedMove);
            _moveStartingTime = DateTime.Now;
        }
        public void PlayerQuit(IPlayer player)
        {
            IPlayer winner = player == _match.Player1 ? _match.Player2 : _match.Player1;
            GameResult gameResult = new GameResult(winner, null, _playedMoves.ToArray(), _startingPlayer, _match, _gameTimeService.GetTotalGameTime());
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
                _moveStartingTime = DateTime.Now;
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
            Field[] line = new Field[4];
            int count = 0;

            line[count++] = new Field(lastPlacedStone.Column, lastPlacedStone.Row);

            for (int rowDown = lastPlacedStone.Row - 1; rowDown >= 0; rowDown--)
            {
                if (_connect4Board[lastPlacedStone.Column][rowDown] != player)
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
        private bool CheckForWinInRow(Field lastPlacedStone, IPlayer player)
        {
            Field[] line = new Field[4];
            int count = 0;

            line[count++] = new Field(lastPlacedStone.Column, lastPlacedStone.Row);

            int col = lastPlacedStone.Column - 1;
            while (col >= 0 && count < 4)
            {
                if (_connect4Board[col][lastPlacedStone.Row] != player)
                    break;

                line[count++] = new Field(col, lastPlacedStone.Row);
                col--;
            }

            col = lastPlacedStone.Column + 1;
            while (col < _connect4Board.Columns && count < 4)
            {
                if (_connect4Board[col][lastPlacedStone.Row] != player)
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
        private bool CheckForWinDiagonallyUp(Field lastPlacedStone, IPlayer player)
        {
            Field[] line = new Field[4];
            int count = 0;

            line[count++] = new Field(lastPlacedStone.Column, lastPlacedStone.Row);

            int col = lastPlacedStone.Column - 1;
            int row = lastPlacedStone.Row - 1;
            while (col >= 0 && row >= 0 && count < 4)
            {
                if (_connect4Board[col][row] != player)
                    break;
                
                line[count++] = new Field(col, row);
                col--;
                row--;
            }

            col = lastPlacedStone.Column + 1;
            row = lastPlacedStone.Row + 1;
            while (col < _connect4Board.Columns && row < _connect4Board.Rows && count < 4)
            {
                if (_connect4Board[col][row] != player)
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
        private bool CheckForWinDiagonallyDown(Field lastPlacedStone, IPlayer player)
        {
            Field[] line = new Field[4];
            int count = 0;

            line[count++] = new Field(lastPlacedStone.Column, lastPlacedStone.Row);

            int col = lastPlacedStone.Column - 1;
            int row = lastPlacedStone.Row + 1;
            while (col >= 0 && row < _connect4Board.Rows && count < 4)
            {
                if (_connect4Board[col][row] != player)
                    break;
                
                line[count++] = new Field(col, row);
                col--;
                row++;
            }

            col = lastPlacedStone.Column + 1;
            row = lastPlacedStone.Row - 1;
            while (col < _connect4Board.Columns && row >= 0 && count < 4)
            {
                if (_connect4Board[col][row] != player)
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
        private void OnConnect4(ICollection<Field> connect4Line, IPlayer player)
        {
            GameResult gameResult = new GameResult(player, connect4Line, _playedMoves.ToArray(), _startingPlayer, _match, _gameTimeService.GetTotalGameTime());
            GameEndet(gameResult);
        }
        private void OnNoMoveLeft()
        {
            GameResult gameResult = new GameResult(null, null, _playedMoves.ToArray(), _startingPlayer, _match, _gameTimeService.GetTotalGameTime());
            GameEndet(gameResult);
        }
        private void GameEndet(GameResult gameResult)
        {
            _match.Player1.GameEnded(gameResult);
            _match.Player2.GameEnded(gameResult);
            _match.Player1.HasConfirmedGameStart = false;
            _match.Player2.HasConfirmedGameStart = false;
            OnGameEnded?.Invoke(gameResult);
        }

        public int GetBestMove(IPlayer player)
        {
            const int LOOK_AHEAD_MOVES = 8;
            const int INVALID_BEST_MOVE = -1;

            IPlayer opponent = _match.Player1 == player ? _match.Player2 : _match.Player1;

            int value = int.MinValue;
            int bestMove = INVALID_BEST_MOVE;
            int alpha = int.MinValue;
            int beta = int.MaxValue;

            foreach (int col in _columnOrder)
            {
                int row;

                if (!GetNextFreeRow(col, out row))
                    continue;

                Debug.Assert(row >= 0);

                _connect4Board[col][row] = player;

                int miniMaxValue;

                if (NoMoveLeft())
                    miniMaxValue = 0;
                else if (HasWon(player, col, row))
                    miniMaxValue = LOOK_AHEAD_MOVES * 1000;
                else if (LOOK_AHEAD_MOVES <= 1)
                    miniMaxValue = CalculateBoardValue(player);
                else
                    miniMaxValue = MiniMax(LOOK_AHEAD_MOVES - 1, player, opponent, false, alpha, beta);

                _connect4Board[col][row] = null;

                if (miniMaxValue > value)
                {
                    bestMove = col;
                    value = miniMaxValue;
                }

                alpha = Math.Max(alpha, value);
                if (alpha >= beta)
                    break;
            }

            Debug.Assert(bestMove != INVALID_BEST_MOVE);
            return bestMove;
        }

        private int MiniMax(int depth, IPlayer maxPlayer, IPlayer minPlayer, bool maximizing, int alpha, int beta)
        {
            int value;

            if (maximizing)
            {
                value = int.MinValue;


                foreach (int col in _columnOrder)
                {
                    int row;

                    if (!GetNextFreeRow(col, out row))
                        continue;

                    Debug.Assert(row >= 0);

                    _connect4Board[col][row] = maxPlayer;

                    int miniMaxValue;

                    if (NoMoveLeft())
                        miniMaxValue = 0;
                    else if (HasWon(maxPlayer, col, row))
                        miniMaxValue = depth * 1000;
                    else if (depth <= 1)
                        miniMaxValue = CalculateBoardValue(maxPlayer);
                    else
                        miniMaxValue = MiniMax(depth - 1, maxPlayer, minPlayer, false, alpha, beta);

                    _connect4Board[col][row] = null;

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
                    int row;

                    if (!GetNextFreeRow(col, out row))
                        continue;

                    Debug.Assert(row >= 0);

                    _connect4Board[col][row] = minPlayer;

                    int miniMaxValue;

                    if (NoMoveLeft())
                        miniMaxValue = 0;
                    else if (HasWon(minPlayer, col, row))
                        miniMaxValue = depth * -1000;
                    else if (depth <= 1)
                        miniMaxValue = CalculateBoardValue(maxPlayer);
                    else
                        miniMaxValue = MiniMax(depth - 1, maxPlayer, minPlayer, true, alpha, beta);

                    _connect4Board[col][row] = null;

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
            while (j < _connect4Board.Rows)
            {
                if (_connect4Board[col][j] == null)
                {
                    row = j;
                    return true;
                }

                j++;
            }

            row = -1;
            return false;
        }
        private int CalculateBoardValue(IPlayer maxPlayer)
        {
            int boardValue = 0;

            for (int i = 0; i < _connect4Board.Columns; i++)
            {
                for (int j = 0; j < _connect4Board.Rows; j++)
                {
                    if (_connect4Board[i][j] == maxPlayer)
                        boardValue += _propabilityMatrix[i][j];
                    else if (_connect4Board[i][j] != null)
                        boardValue -= _propabilityMatrix[i][j];
                    else
                        break;
                }
            }

            return boardValue;
        }
        private bool NoMoveLeft()
        {
            for (int i = 0; i < _connect4Board.Columns; i++)
                if (_connect4Board[i][_connect4Board.Rows - 1] == null)
                    return false;

            return true;
        }
        private bool HasWon(IPlayer player, int col, int row)
        {
            // vertically

            int count = 1;
            int j = row - 1;
            while (j >= 0)
            {
                if (_connect4Board[col][j] != player)
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
                if (_connect4Board[i][row] != player)
                    break;

                i--;
                count++;
            }
            i = col + 1;
            while(i < _connect4Board.Columns)
            {
                if (_connect4Board[i][row] != player)
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
                if (_connect4Board[i][j] != player)
                    break;

                i--;
                j--;
                count++;
            }
            i = col + 1;
            j = row + 1;
            while (i < _connect4Board.Columns && j < _connect4Board.Rows)
            {
                if (_connect4Board[i][j] != player)
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
            while (i >= 0 && j < _connect4Board.Rows)
            {
                if (_connect4Board[i][j] != player)
                    break;

                i--;
                j++;
                count++;
            }
            i = col + 1;
            j = row - 1;
            while (i < _connect4Board.Columns && j >= 0)
            {
                if (_connect4Board[i][j] != player)
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
        private readonly GameTimeService _gameTimeService;
        private bool _disposed = false;
        private IPlayer _activePlayer;
        private readonly IPlayer _startingPlayer;
        private readonly Match _match;
        private readonly Connect4Board _connect4Board;
        private readonly ICollection<PlayedMove> _playedMoves = new List<PlayedMove>();
        private readonly int[] _columnOrder = { 3, 2, 4, 1, 5, 0, 6 };
        private readonly int[][] _propabilityMatrix = [[3, 4, 5, 5, 4, 3],
                                                       [4, 6, 8, 8, 6, 4],
                                                       [5, 8, 11, 11, 8, 5],
                                                       [7, 10, 13, 13, 10, 7],
                                                       [5, 8, 11, 11, 8, 5],
                                                       [4, 6, 8, 8, 6, 4],
                                                       [3, 4, 5, 5, 4, 3]];
    }
}

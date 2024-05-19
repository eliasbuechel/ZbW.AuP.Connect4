using System.Diagnostics;

namespace backend.game
{
    internal class GameLogic
    {
        public GameLogic(IPlayer?[][] boardState, IPlayer activePlayer, IPlayer opponent)
        {
            _boardState = boardState;
            _activePlayer = activePlayer;
            _opponent = opponent;
        }

        public int GetBestMove()
        {
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

                _boardState[col][row] = _activePlayer;

                int miniMaxValue;

                if (NoMoveLeft())
                    miniMaxValue = 0;
                else if (HasWon(_activePlayer, col, row))
                    miniMaxValue = LOOK_AHEAD_MOVES * 1000;
                else if (LOOK_AHEAD_MOVES <= 1)
                    miniMaxValue = CalculateBoardValue(_activePlayer);
                else
                    miniMaxValue = MiniMax(LOOK_AHEAD_MOVES - 1, _activePlayer, _opponent, false, alpha, beta);

                _boardState[col][row] = null;

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

                    _boardState[col][row] = maxPlayer;

                    int miniMaxValue;

                    if (NoMoveLeft())
                        miniMaxValue = 0;
                    else if (HasWon(maxPlayer, col, row))
                        miniMaxValue = depth * 1000;
                    else if (depth <= 1)
                        miniMaxValue = CalculateBoardValue(maxPlayer);
                    else
                        miniMaxValue = MiniMax(depth - 1, maxPlayer, minPlayer, false, alpha, beta);

                    _boardState[col][row] = null;

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

                    _boardState[col][row] = minPlayer;

                    int miniMaxValue;

                    if (NoMoveLeft())
                        miniMaxValue = 0;
                    else if (HasWon(minPlayer, col, row))
                        miniMaxValue = depth * -1000;
                    else if (depth <= 1)
                        miniMaxValue = CalculateBoardValue(maxPlayer);
                    else
                        miniMaxValue = MiniMax(depth - 1, maxPlayer, minPlayer, true, alpha, beta);

                    _boardState[col][row] = null;

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
            while (j < _boardState[col].Length)
            {
                if (_boardState[col][j] == null)
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

            for (int i = 0; i < _boardState.Length; i++)
            {
                for (int j = 0; j < _boardState[i].Length; j++)
                {
                    if (_boardState[i][j] == maxPlayer)
                        boardValue += _propabilityMatrix[i][j];
                    else if (_boardState[i][j] != null)
                        boardValue -= _propabilityMatrix[i][j];
                    else
                        break;
                }
            }

            return boardValue;
        }
        private bool NoMoveLeft()
        {
            for (int i = 0; i < _boardState.Length; i++)
                if (_boardState[i][_boardState[i].Length - 1] == null)
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
                if (_boardState[col][j] != player)
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
                if (_boardState[i][row] != player)
                    break;

                i--;
                count++;
            }
            i = col + 1;
            while (i < _boardState.Length)
            {
                if (_boardState[i][row] != player)
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
                if (_boardState[i][j] != player)
                    break;

                i--;
                j--;
                count++;
            }
            i = col + 1;
            j = row + 1;
            while (i < _boardState.Length && j < _boardState[i].Length)
            {
                if (_boardState[i][j] != player)
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
            while (i >= 0 && j < _boardState[i].Length)
            {
                if (_boardState[i][j] != player)
                    break;

                i--;
                j++;
                count++;
            }
            i = col + 1;
            j = row - 1;
            while (i < _boardState.Length && j >= 0)
            {
                if (_boardState[i][j] != player)
                    break;

                i++;
                j--;
                count++;
            }
            if (count >= 4)
                return true;

            return false;
        }

        private const int LOOK_AHEAD_MOVES = 13;
        private const int INVALID_BEST_MOVE = -1;

        private readonly IPlayer?[][] _boardState;
        private readonly IPlayer _activePlayer;
        private readonly IPlayer _opponent;
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

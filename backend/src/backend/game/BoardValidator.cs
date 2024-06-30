using backend.game.entities;
using backend.game.players;
using backend.infrastructure;
using System.Diagnostics;
using System.Numerics;

namespace backend.game
{
    internal class BoardValidator(Player?[][] gameBoard)
    {
        public bool HasWon(Player player, Field field)
        {
            return HasWonVertically(player, field) ||
                HasWonHorzontally(player, field) ||
                HasWonDiagonallyUp(player, field) ||
                HasWonDiagonallyDown(player, field);
        }
        public bool HasNoMoveLeft()
        {
            for (int i = 0; i < Board.Columns; i++)
                if (_gameBoard[i][Board.Rows - 1] == null)
                    return false;

            return true;
        }
        public int CalculateBoardValue(Player maximizingPlayer)
        {
            int boardValue = 0;

            for (int i = 0; i < Board.Columns; i++)
            {
                for (int j = 0; j < Board.Rows; j++)
                {
                    if (_gameBoard[i][j] == maximizingPlayer)
                        boardValue += PROPABILITY_MATRIX[i][j];
                    else if (_gameBoard[i][j] != null)
                        boardValue -= PROPABILITY_MATRIX[i][j];
                    else
                        break;
                }
            }

            return boardValue;
        }
        public bool GetNextFreeRow(int col, out int row)
        {
            int j = 0;
            while (j < Board.Rows)
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
        public int GetBestMove(Player player, Player opponent)
        {
            int value = int.MinValue;
            int bestMove = INVALID_BEST_MOVE;
            int alpha = int.MinValue;
            int beta = int.MaxValue;

            foreach (int col in COLUMN_ORDER)
            {
                if (!GetNextFreeRow(col, out int row))
                    continue;

                Debug.Assert(row >= 0);

                _gameBoard[col][row] = player;

                int miniMaxValue;

                if (HasNoMoveLeft())
                    miniMaxValue = 0;
                else if (HasWon(player, new(col, row)))
                    miniMaxValue = LOOK_AHEAD_MOVES * 1000;
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
        public void CheckForWin(Field field, Player player, Action onNoMoveLeft, Action<ICollection<Field>> onConnect4)
        {
            if (CheckForWinInColumn(field, player, onConnect4))
                return;
            if (CheckForWinInRow(field, player, onConnect4))
                return;
            if (CheckForWinDiagonallyUp(field, player, onConnect4))
                return;
            if (CheckForWinDiagonallyDown(field, player, onConnect4))
                return;
            if (HasNoMoveLeft())
                onNoMoveLeft();
        }

        private bool CheckForWinInColumn(Field lastPlacedStone, Player player, Action<ICollection<Field>> onConnect4)
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
                    onConnect4(line);
                    return true;
                }
            }

            return false;
        }
        private bool CheckForWinInRow(Field lastPlacedStone, Player player, Action<ICollection<Field>> onConnect4)
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
            while (col < Board.Columns && count < 4)
            {
                if (_gameBoard[col][lastPlacedStone.Row] != player)
                    break;

                line[count++] = new Field(col, lastPlacedStone.Row);
                col++;
            }

            if (count >= 4)
            {
                onConnect4(line);
                return true;
            }

            return false;
        }
        private bool CheckForWinDiagonallyUp(Field lastPlacedStone, Player player, Action<ICollection<Field>> onConnect4)
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
            while (col < Board.Columns && row < Board.Rows && count < 4)
            {
                if (_gameBoard[col][row] != player)
                    break;

                line[count++] = new Field(col, row);
                col++;
                row++;
            }

            if (count >= 4)
            {
                onConnect4(line);
                return true;
            }

            return false;
        }
        private bool CheckForWinDiagonallyDown(Field lastPlacedStone, Player player, Action<ICollection<Field>> onConnect4)
        {
            Field[] line = new Field[4];
            int count = 0;

            line[count++] = new Field(lastPlacedStone.Column, lastPlacedStone.Row);

            int col = lastPlacedStone.Column - 1;
            int row = lastPlacedStone.Row + 1;
            while (col >= 0 && row < Board.Rows && count < 4)
            {
                if (_gameBoard[col][row] != player)
                    break;

                line[count++] = new Field(col, row);
                col--;
                row++;
            }

            col = lastPlacedStone.Column + 1;
            row = lastPlacedStone.Row - 1;
            while (col < Board.Columns && row >= 0 && count < 4)
            {
                if (_gameBoard[col][row] != player)
                    break;

                line[count++] = new Field(col, row);
                col++;
                row--;
            }

            if (count >= 4)
            {
                onConnect4(line);
                return true;
            }

            return false;
        }


        private int MiniMax(int depth, Player maxPlayer, Player minPlayer, bool maximizing, int alpha, int beta)
        {
            if (maximizing)
                return Maximizing(depth, maxPlayer, minPlayer, alpha, beta);

            return Minimizing(depth, maxPlayer, minPlayer, alpha, beta);
        }
        private int Maximizing(int depth, Player maxPlayer, Player minPlayer, int alpha, int beta)
        {
            int max = int.MinValue;

            foreach (int col in COLUMN_ORDER)
            {
                if (!GetNextFreeRow(col, out int row))
                    continue;

                _gameBoard[col][row] = maxPlayer;

                int miniMaxValue;

                if (HasNoMoveLeft())
                    miniMaxValue = 0;
                else if (HasWon(maxPlayer, new(col, row)))
                    miniMaxValue = depth * 1000;
                else if (depth <= 1)
                    miniMaxValue = CalculateBoardValue(maxPlayer);
                else
                    miniMaxValue = MiniMax(depth - 1, maxPlayer, minPlayer, false, alpha, beta);
                _gameBoard[col][row] = null;

                max = Math.Max(max, miniMaxValue);
                alpha = Math.Max(alpha, max);
                if (alpha >= beta)
                    break;
            }

            return max;
        }
        private int Minimizing(int depth, Player maxPlayer, Player minPlayer, int alpha, int beta)
        {
            int min = int.MaxValue;

            foreach (int col in COLUMN_ORDER)
            {
                if (!GetNextFreeRow(col, out int row))
                    continue;

                _gameBoard[col][row] = minPlayer;

                int miniMaxValue;

                if (HasNoMoveLeft())
                    miniMaxValue = 0;
                else if (HasWon(minPlayer, new(col, row)))
                    miniMaxValue = depth * -1000;
                else if (depth <= 1)
                    miniMaxValue = CalculateBoardValue(maxPlayer);
                else
                    miniMaxValue = MiniMax(depth - 1, maxPlayer, minPlayer, true, alpha, beta);

                _gameBoard[col][row] = null;

                min = Math.Min(min, miniMaxValue);
                beta = Math.Min(beta, min);
                if (alpha >= beta)
                    break;
            }

            return min;
        }

        private bool HasWonVertically(Player player, Field field)
        {
            int count = 1;
            int j = field.Row - 1;

            while (j >= 0)
            {
                if (_gameBoard[field.Column][j] != player)
                    break;

                j--;
                count++;
            }

            return count >= 4;
        }
        private bool HasWonHorzontally(Player player, Field field)
        {
            int count = 1;
            int i = field.Column - 1;
            while (i >= 0)
            {
                if (_gameBoard[i][field.Row] != player)
                    break;

                i--;
                count++;
            }
            i = field.Column + 1;
            while (i < Board.Columns)
            {
                if (_gameBoard[i][field.Row] != player)
                    break;

                i++;
                count++;
            }

            return count >= 4;
        }
        private bool HasWonDiagonallyUp(Player player, Field field)
        {
            int count = 1;
            int i = field.Column - 1;
            int j = field.Row - 1;
            while (i >= 0 && j >= 0)
            {
                if (_gameBoard[i][j] != player)
                    break;

                i--;
                j--;
                count++;
            }
            i = field.Column + 1;
            j = field.Row + 1;
            while (i < Board.Columns && j < Board.Rows)
            {
                if (_gameBoard[i][j] != player)
                    break;

                i++;
                j++;
                count++;
            }
            
            return count >= 4;
        }
        private bool HasWonDiagonallyDown(Player player, Field field)
        {
            int count = 1;
            int i = field.Column - 1;
            int j = field.Row + 1;
            while (i >= 0 && j < Board.Rows)
            {
                if (_gameBoard[i][j] != player)
                    break;

                i--;
                j++;
                count++;
            }
            i = field.Column + 1;
            j = field.Row - 1;
            while (i < Board.Columns && j >= 0)
            {
                if (_gameBoard[i][j] != player)
                    break;

                i++;
                j--;
                count++;
            }

            return count >= 4;
        }

        private readonly Player?[][] _gameBoard = gameBoard;

        private const int INVALID_BEST_MOVE = -1;
        private const int LOOK_AHEAD_MOVES = 10;
        private static readonly int[][] PROPABILITY_MATRIX = [[3, 4, 5, 5, 4, 3], [4, 6, 8, 8, 6, 4], [5, 8, 11, 11, 8, 5], [7, 10, 13, 13, 10, 7], [5, 8, 11, 11, 8, 5], [4, 6, 8, 8, 6, 4], [3, 4, 5, 5, 4, 3]];
        private static readonly int[] COLUMN_ORDER = [3, 2, 4, 1, 5, 0, 6];
    }
}

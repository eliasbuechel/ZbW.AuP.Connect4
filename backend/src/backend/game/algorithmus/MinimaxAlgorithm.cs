

namespace backend.game.algorithmus
{
    internal class MinimaxAlgorithm
    {
        public int GetBestMove(IPlayer?[][] board, int depth, bool maximizingPlayer)
        {
            int bestScore = maximizingPlayer ? int.MinValue : int.MaxValue;
            int bestMove = -1;

            for (int column = 0; column < board.Length; column++)
            {
                if (IsColumnFull(board, column))
                    continue;

                IPlayer?[][] newBoard = DeepCopy(board);
                int row = DropStone(newBoard, column, maximizingPlayer ? 1 : 2); // Assuming 1 for maximizing player and 2 for minimizing player
                int score = maximizingPlayer ? Minimax(newBoard, depth - 1, false) : Minimax(newBoard, depth - 1, true);

                if ((maximizingPlayer && score > bestScore) || (!maximizingPlayer && score < bestScore))
                {
                    bestScore = score;
                    bestMove = column;
                }
            }

            return bestMove;
        }

        private int Minimax(IPlayer?[][] board, int depth, bool maximizingPlayer)
        {
            // Implement Minimax algorithm here
            // This is just a placeholder
            return 0;
        }

        private bool IsColumnFull(IPlayer?[][] board, int column)
        {
            return board[column][0] != null;
        }

        private int DropStone(IPlayer?[][] board, int column, int player)
        {
            for (int row = board[column].Length - 1; row >= 0; row--)
            {
                if (board[column][row] == null)
                {
                    board[column][row] = new Player(player); // Assuming Player class exists
                    return row;
                }
            }
            return -1; // Column is full
        }

        private IPlayer?[][] DeepCopy(IPlayer?[][] original)
        {
            IPlayer?[][] copy = new IPlayer?[original.Length][];
            for (int i = 0; i < original.Length; i++)
            {
                copy[i] = (IPlayer?[])original[i].Clone();
            }
            return copy;
        }

        public double[,] winningOpportunityValues = {
        {0.03, 0.04, 0.05, 0.07, 0.05, 0.04, 0.03},
        {0.04, 0.06, 0.08, 0.10, 0.08, 0.06, 0.04},
        {0.05, 0.08, 0.11, 0.13, 0.11, 0.08, 0.05},
        {0.05, 0.08, 0.11, 0.13, 0.11, 0.08, 0.05},
        {0.04, 0.06, 0.08, 0.10, 0.08, 0.06, 0.04},
        {0.03, 0.04, 0.05, 0.07, 0.05, 0.04, 0.03}
        };


    }
}

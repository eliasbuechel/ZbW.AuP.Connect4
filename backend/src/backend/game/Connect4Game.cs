using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

namespace backend.game
{
    internal class Connect4Game
    {
        public Connect4Game(Match match)
        {
            _activePlayer = match.Player1;
            _match = match;

            match.Player1.GameStarted(this);
            match.Player2.GameStarted(this);
        }

        public Guid Id { get; } = new Guid();
        public Match Match => _match;
        public IPlayer ActivePlayer => _activePlayer;
        public string[][] FieldAsIds => _connect4Board.FieldAsIds;


        public bool PlayMove(IPlayer player, int column)
        {
            Debug.Assert(_activePlayer == player);
            Debug.Assert(_connect4Board.PlaceStone(player, column));
            Field? lastPlacedStone = _connect4Board.LastPlacedStone;
            Debug.Assert(lastPlacedStone != null);

            GameResult? gameResult = CheckForWin(player, lastPlacedStone);

            SwapActivePlayer();
            _activePlayer.MovePlayed(column);

            if (gameResult != null)
            {
                Match.Player1.GameEnded(gameResult);
                Match.Player2.GameEnded(gameResult);
                return false;
            }

            return true;
        }
        private void SwapActivePlayer()
        {
            _activePlayer = _activePlayer == _match.Player1 ? _match.Player2 : _match.Player1;
        }
        public void Quit(Player player)
        {
            IPlayer winner = player == _match.Player1 ? _match.Player2 : _match.Player1;
            GameResult gameResult = new GameResult(winner, null);
            _match.Player1.GameEnded(gameResult);
            _match.Player2.GameEnded(gameResult);
        }

        private GameResult? CheckForWin(IPlayer player, Field field)
        {
            Connect4Line? connect4Line;

            connect4Line = CheckForWinInColumn(player, field);
            if (connect4Line != null)
                return new GameResult(player, connect4Line);

            connect4Line = CheckForWinInRow(player, field);
            if (connect4Line != null)
                return new GameResult(player, connect4Line);

            connect4Line = CheckForWinDiagonally(player, field);
            if (connect4Line != null)
                return new GameResult(player, connect4Line);

            return null;
        }
        private Connect4Line? CheckForWinInColumn(IPlayer player, Field lastPlacedStone)
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
                    return connect4Line;
                }
            }

            return null;
        }
        private Connect4Line? CheckForWinInRow(IPlayer player, Field lastPlacedStone)
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
                    return connect4Line;
                }
            }

            return null;
        }
        private Connect4Line? CheckForWinDiagonally(IPlayer player, Field lastPlacedStone)
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
                    return connect4Line;
                }

                c++;
                r++;
            }

            return null;
        }


        private IPlayer _activePlayer;
        private readonly Match _match;
        private readonly Connect4Board _connect4Board = new Connect4Board();
    }
}

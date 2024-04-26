using System.Diagnostics;

namespace backend.game
{
    internal class Connect4Game
    {
        public Connect4Game(Match match)
        {
            _activePlayer = match.Player1;
            _match = match;

            match.Player1.GameStarted();
            match.Player2.GameStarted();
        }

        public Guid Id { get; } = new Guid();
        public Match Match => _match;
        public IPlayer ActivePlayer => _activePlayer;
        public string[][] FieldAsIds => _connect4Board.FieldAsIds;


        public void PlayMove(IPlayer player, int column)
        {
            Debug.Assert(_activePlayer == player);
            Debug.Assert(_connect4Board.PlaceStone(player, column));
            SwapActivePlayer();
            _activePlayer.MovePlayed(column);

            //if (_lastplayer != null && _lastplayer == player)
            //    throw new playermanager("its the other players turn.", player);

            //if (player != _player1 && player != _player2)
            //    throw new playermanager("player is not in the current game.", player);

            //if (column < 0 || column >= columns)
            //    throw new InvalidArg("provided column is not valid", player, column);


            //int nextFreeRow = -1;
            //for (int row = 0; row < ROWS; row++)
            //{
            //    if (_field[column][row] == null)
            //    {
            //        nextFreeRow = row;
            //        break;
            //    }
            //}

            //if (nextFreeRow == -1)
            //    throw new MoveNotPossibleException($"Column is already full!", player, column);

            //_field[column][nextFreeRow] = player;
            //_lastPlayer = player;
            //OnMovePlayed?.Invoke(player, column);

            //CheckForWin(player, column, nextFreeRow);
        }

        private void SwapActivePlayer()
        {
            _activePlayer = _activePlayer == _match.Player1 ? _match.Player2 : _match.Player1;
        }

        internal void Quit(Player player)
        {
            IPlayer winner = player == _match.Player1 ? _match.Player2 : _match.Player1;
            GameResult gameResult = new GameResult(winner, null);

            winner.GameEnded(gameResult);
        }

        //private void CheckForWin(IPlayer player, int column, int row)
        //{
        //    if (CheckForWinInColumn(player, column, row))
        //        return;

        //    if (CheckForWinInRow(player, column, row))
        //        return;

        //    if (CheckForWinDiagonally(player, column, row))
        //        return;
        //}
        //private bool CheckForWinInColumn(IPlayer player, int column, int row)
        //{
        //    Connect4Line connect4Line = new Connect4Line();
        //    int count = 4;

        //    count--;
        //    connect4Line[count].Column = column;
        //    connect4Line[count].Row = row;

        //    for (int rowDown = row - 1; rowDown >= 0; rowDown--)
        //    {
        //        if (_field[column][rowDown] != player)
        //            break;

        //        count--;
        //        connect4Line[count].Column = column;
        //        connect4Line[count].Row = rowDown;

        //        if (count == 0)
        //        {
        //            OnConnect4?.Invoke(connect4Line);
        //            return true;
        //        }
        //    }

        //    return false;
        //}
        //private bool CheckForWinInRow(IPlayer player, int column, int row)
        //{
        //    Connect4Line connect4Line = new Connect4Line();
        //    int count = 0;

        //    for (int i = 0; i < COLUMNS; i++)
        //    {
        //        if (_field[i][row] == player)
        //        {
        //            connect4Line[count].Column = i;
        //            connect4Line[count].Row = row;
        //            count++;
        //        }
        //        else
        //            count = 0;

        //        if (count == 4)
        //        {
        //            OnConnect4?.Invoke(connect4Line);
        //            return true;
        //        }
        //    }

        //    return false;
        //}
        //private bool CheckForWinDiagonally(IPlayer player, int column, int row)
        //{
        //    Connect4Line connect4Line = new Connect4Line();
        //    int c = column;
        //    int r = row;

        //    while (true)
        //    {
        //        if (c <= 0 || r <= 0)
        //            break;

        //        c--;
        //        r--;
        //    }

        //    int count = 0;
        //    while (c < COLUMNS && r < ROWS)
        //    {
        //        if (_field[c][r] == player)
        //        {
        //            connect4Line[count].Column = c;
        //            connect4Line[count].Row = r;
        //            count++;
        //        }
        //        else
        //            count = 0;

        //        if (count == 4)
        //        {
        //            OnConnect4?.Invoke(connect4Line);
        //            return true;
        //        }

        //        c++;
        //        r++;
        //    }

        //    return false;
        //}


        private IPlayer _activePlayer;
        private readonly Match _match;
        private readonly Connect4Board _connect4Board = new Connect4Board();
    }
}

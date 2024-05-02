using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;

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

            _activePlayer = match.Player1;
        }

        public event Action? OnGameEnded;

        public Guid Id { get; } = new Guid();
        public Match Match => _match;
        public IPlayer ActivePlayer => _activePlayer;
        public string[][] FieldAsIds => _connect4Board.FieldAsIds;

        public void PlayMove(IPlayer player, int column)
        {
            if (_activePlayer != player)
            {
                Debug.Assert(false);
                return;
            }
            
            Debug.Assert(_connect4Board.PlaceStone(player, column));
        }
        public void PlayerQuit(IPlayer player)
        {
            IPlayer winner = player == _match.Player1 ? _match.Player2 : _match.Player1;
            GameResult gameResult = new GameResult(winner, null);
            _match.Player1.GameEnded(gameResult);
            _match.Player2.GameEnded(gameResult);
            OnGameEnded?.Invoke();
        }
        public void Initialize()
        {
            _connect4Board.Reset();
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
            if (CheckForWinDiagonally(field))
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
        private bool CheckForWinDiagonally(Field lastPlacedStone)
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
        private void OnConnect4(Connect4Line connect4Line)
        {
            GameResult gameResult = new GameResult(_activePlayer, connect4Line);
            _match.Player1.GameEnded(gameResult);
            _match.Player2.GameEnded(gameResult);

            OnGameEnded?.Invoke();
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

        private bool _disposed = false;
        private IPlayer _activePlayer;
        private readonly Match _match;
        private readonly Connect4Board _connect4Board;
    }
}

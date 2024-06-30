using backend.communication.mqtt;
using backend.game.entities;
using backend.game.players;
using backend.infrastructure;
using backend.utilities;

namespace backend.game
{
    internal class Board
    {
        public Board(RoboterApi roboterAPI)
        {
            _gameBoard = new Player?[COLUMNS][];

            for (int i = 0; i < _gameBoard.Length; i++)
                _gameBoard[i] = new Player?[ROWS];
            _roboterAPI = roboterAPI;

            _roboterAPI.OnStonePlaced += OnStonePlacedOnRoboter;
            _roboterAPI.OnBoardReset += OnRoboterBoardReset;
        }
        ~Board()
        {
            _roboterAPI.OnStonePlaced -= OnStonePlacedOnRoboter;
            _roboterAPI.OnBoardReset -= OnRoboterBoardReset;
        }

        public event Action<Player, Field>? OnStonePlaced;
        public event Action? OnBoardReset;

        public string[][] FieldAsIds
        {
            get
            {
                string[][] fieldAsIds = new string[_gameBoard.Length][];

                for (int i = 0; i < _gameBoard.Length; i++)
                {
                    fieldAsIds[i] = new string[_gameBoard[i].Length];

                    for (int j = 0; j < _gameBoard[i].Length; j++)
                    {
                        Player? player = _gameBoard[i][j];
                        fieldAsIds[i][j] = player == null ? "" : player.Id;
                    }
                }

                return fieldAsIds;
            }
        }
        public static int Columns => COLUMNS;
        public static int Rows => ROWS;
        public Player?[][] GameBoard => _gameBoard;
        public Field? PlacingField { get; private set; }
        public bool IsVisualizingOnRoboter => _roboterAPI.IsVisualizingOnRoboter;


        public Player?[] this[int index]
        {
            get { return _gameBoard[index]; }
        }

        public void PlaceStone(Player player, int column)
        {
            if (column < 0 || column >= COLUMNS)
                throw new InvalidPlayerRequestException($"Exception while playing move. {column} is not a valid column for a move.");

            Player?[] col = _gameBoard[column];
            if (col[^1] != null)
                throw new InvalidPlayerRequestException($"Exception while playing move. Cannot play move in full column {column}.");

            for (int i = 0; i < col.Length; i++)
            {
                if (col[i] != null)
                    continue;

                col[i] = player;
                Field field = new(column, i);
                PlacingField = field;
                _roboterAPI.PlaceStone(player, field);
                return;
            }

            throw new InvalidPlayerRequestException("Exception while playing move. Something went wrong while placing stone.");
        }
        public void Reset()
        {
            for (int i = 0; i < _gameBoard.Length; i++)
                for (int j = 0 ; j < _gameBoard[i].Length; j++)
                    _gameBoard[i][j] = null;

            _roboterAPI.ResetConnect4Board();
        }

        private void OnStonePlacedOnRoboter(Player player, Field field)
        {
            Logger.Log(LogLevel.Debug, LogContext.GAME_PLAY, $"Invoke playing move on board. Player: {player.Username} Column: {field.Column}");
            OnStonePlaced?.Invoke(player, field);
            PlacingField = null;
        }
        private void OnRoboterBoardReset()
        {
            OnBoardReset?.Invoke();
        }

        private readonly Player?[][] _gameBoard;
        private readonly RoboterApi _roboterAPI;
        private const int ROWS = 6;
        private const int COLUMNS = 7;
    }
}

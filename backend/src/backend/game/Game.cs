using backend.game.entities;
using backend.game.players;
using backend.infrastructure;
using backend.utilities;
using System.Diagnostics;

namespace backend.game
{
    internal class Game : DisposingObject
    {
        public Game(Match match, Board board)
        {
            _match = match;
            _board = board;

            _board.OnStonePlaced += OnStonePlaced;
            _board.OnBoardReset += OnBoardReset;


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
        public event Func<Player, Field, Task>? OnMovePlayed;
        public event Action<Game>? OnGameStarted;

        public Match Match => _match;
        public Player ActivePlayer => _activePlayer;
        public string[][] FieldAsIds => _board.FieldAsIds;
        public bool GameEnded => _gameEnded;
        public DateTime? MoveStartTime => _moveStartingTime;
        public DateTime? GameStartTime => _gameStartTime;
        public Board Board => _board;


        public Field? PlacingField => _board.PlacingField;
        public Field? LastPlacedStone { get; private set; }
        public bool IsQuittableByEveryone => _match.Player1 is OpponentRoboterPlayer || _match.Player2 is OpponentRoboterPlayer;

        public void GameStartGotConfirmed()
        {
            if (_match.Player1.HasConfirmedGameStart && _match.Player2.HasConfirmedGameStart)
            {
                _gameStartTime = DateTime.Now;
                _moveStartingTime = DateTime.Now;
                _moveDurationStopWatch.Restart();
                _match.Player1.TotalPlayTime = new TimeSpan(0);
                _match.Player2.TotalPlayTime = new TimeSpan(0);
            }
        }
        public void PlayMove(Player player, int column)
        {
            if (_activePlayer != player)
                throw new InvalidPlayerRequestException($"Play move exception [player:{player.Username} column:{column}]. Not the players turn!");

            if (_activePlayerPlacedStone)
                throw new InvalidPlayerRequestException($"Play move exception [player:{player.Username} column:{column}]. Already played move in his turn!");

            if (column < 0 || column > Board.Columns)
                throw new InvalidPlayerRequestException($"Play move exception [player:{player.Username} column:{column}]. Column is not in valid range");

            _board.PlaceStone(player, column);

            //Debug.Assert(_moveStartingTime != null);

            PlayedMove playedMove = new(column, _moveDurationStopWatch.Elapsed);

            _activePlayer.TotalPlayTime = _activePlayer.TotalPlayTime == null ? _moveDurationStopWatch.Elapsed : _activePlayer.TotalPlayTime.Value + _moveDurationStopWatch.Elapsed;

            _playedMoves.Add(playedMove);
            _moveStartingTime = DateTime.Now;
            _moveDurationStopWatch.Restart();
            _activePlayerPlacedStone = true;
        }
        public void PlayerQuit(Player player)
        {
            Player quittingPlayer = player;
            if (_match.Player1 is OpponentRoboterPlayer && _match.Player2 is AlgorythmPlayer algorythmPlayer2)
                quittingPlayer = algorythmPlayer2;
            else if (_match.Player2 is OpponentRoboterPlayer && _match.Player1 is AlgorythmPlayer algorythmPlayer1)
                quittingPlayer = algorythmPlayer1;
            else if (!_match.Player1.Equals(player) && !_match.Player2.Equals(player))
                throw new InvalidPlayerRequestException($"Quit game exception [player:{player.Username}]. Quitting player is not part of the active game.");

            Player winner = quittingPlayer == _match.Player1 ? _match.Player2 : _match.Player1;
            GameResult gameResult = new(winner, null, _playedMoves.ToArray(), _startingPlayer, _match);

            Logger.Log(LogLevel.Debug, LogContext.GAME_PLAY, $"Player quit during game. Player: {player.Username}");
            OnGameEndet(gameResult);
        }
        public void Initialize()
        {
            _board.Reset();
        }
        public int GetBestMove(Player player)
        {
            BoardValidator validator = new(_board.GameBoard);

            Player opponent = _match.Player1 == player ? _match.Player2 : _match.Player1;
            return validator.GetBestMove(player, opponent);
        }
        public void PlayManualMove(int column)
        {
            if (_gameEnded) return;
            PlayMove(_activePlayer, column);
        }

        protected override void OnDispose()
        {
            _board.OnStonePlaced -= OnStonePlaced;
            _board.OnBoardReset -= OnBoardReset;
        }

        private void OnBoardReset()
        {
            OnGameStarted?.Invoke(this);
        }
        private async void OnStonePlaced(Player player, Field field)
        {
            SwapActivePlayer();
            LastPlacedStone = field;
            Logger.Log(LogLevel.Debug, LogContext.GAME_PLAY, $"Invoke playing move in game. Player: {player.Username} Column: {field.Column}");

            BoardValidator validator = new(_board.GameBoard);
            validator.CheckForWin(field, player, OnNoMoveLeft, (line) => OnConnect4(line, player));

            if (OnMovePlayed != null)
                await OnMovePlayed.Invoke(player, field);
        }
        private void SwapActivePlayer()
        {
            _activePlayer = _activePlayer == _match.Player1 ? _match.Player2 : _match.Player1;
            _activePlayerPlacedStone = false;
        }
        private void OnConnect4(ICollection<Field> connect4Line, Player player)
        {
            GameResult gameResult = new(player, connect4Line, _playedMoves.ToArray(), _startingPlayer, _match);

            Logger.Log(LogLevel.Debug, LogContext.GAME_PLAY, $"Game ended with connect4. Player: {player.Username}");
            OnGameEndet(gameResult);
        }
        private void OnNoMoveLeft()
        {
            GameResult gameResult = new(null, null, _playedMoves.ToArray(), _startingPlayer, _match);

            Logger.Log(LogLevel.Debug, LogContext.GAME_PLAY, $"Game ended with no move left.");
            OnGameEndet(gameResult);
        }
        private void OnGameEndet(GameResult gameResult)
        {
            _match.Player1.HasConfirmedGameStart = false;
            _match.Player2.HasConfirmedGameStart = false;

            _gameEnded = true;
            OnGameEnded?.Invoke(gameResult);
        }


        private DateTime? _moveStartingTime;
        private readonly Stopwatch _moveDurationStopWatch = new();
        private DateTime? _gameStartTime;
        private bool _gameEnded;
        private Player _activePlayer;
        private bool _activePlayerPlacedStone;
        private readonly Player _startingPlayer;
        private readonly Match _match;
        private readonly Board _board;
        private readonly List<PlayedMove> _playedMoves = [];
    }
}

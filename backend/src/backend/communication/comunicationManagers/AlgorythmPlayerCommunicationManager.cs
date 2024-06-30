using backend.infrastructure;
using backend.services;
using backend.game.entities;
using backend.game;
using backend.game.players;
using backend.utilities;

namespace backend.communication.comunicationManagers
{
    internal class AlgorythmPlayerCommunicationManager : DisposingObject
    {
        public AlgorythmPlayerCommunicationManager(GameManager gameManager)
        {
            _gameManager = gameManager;

            _gameManager.OnRequestedMatch += OnRequestedMatch;
            _gameManager.OnGameStarted += OnGameStarted;
            _gameManager.OnConfirmedGameStart += OnConfirmedGameStart;
            _gameManager.OnMovePlayed += OnMovePlayed;
        }

        private void OnMovePlayed(Player player, Field field)
        {
            if (player != _opponentPlayer)
                return;

            if (_algorythmPlayer == null)
                return;

            if (_gameManager.Game == null || _gameManager.Game.GameEnded)
                return;

            int column = _gameManager.GetBestMove(_algorythmPlayer);
            ExecutePlayerRequest(() => _gameManager.PlayMove(_algorythmPlayer, column));
        }
        private void OnRequestedMatch(Player requester, Player opponent)
        {
            if (opponent is AlgorythmPlayer && requester is WebPlayer)
                ExecutePlayerRequest(() => _gameManager.AcceptMatch(opponent, requester));
        }
        private void OnGameStarted(Game game)
        {
            if (game.Match.Player1 is AlgorythmPlayer algorythmPlayer1)
            {
                if (game.Match.Player1 == game.ActivePlayer)
                    _algorythmPlayerIsStartingPlayer = true;

                _opponentPlayer = game.Match.Player2;
                _algorythmPlayer = algorythmPlayer1;
                ExecutePlayerRequest(() => _gameManager.ConfirmGameStart(algorythmPlayer1));
            }

            if (game.Match.Player2 is AlgorythmPlayer algorythmPlayer2)
            {
                if (game.Match.Player2 == game.ActivePlayer)
                    _algorythmPlayerIsStartingPlayer = true;

                _opponentPlayer = game.Match.Player1;
                _algorythmPlayer = algorythmPlayer2;
                ExecutePlayerRequest(() => _gameManager.ConfirmGameStart(algorythmPlayer2));
            }
        }
        private void OnConfirmedGameStart(Player player)
        {
            if (_opponentPlayer == null || _algorythmPlayer == null)
                return;

            if (player != _algorythmPlayer)
                return;

            if (!_algorythmPlayerIsStartingPlayer)
                return;

            _algorythmPlayerIsStartingPlayer = false;
            int column = _gameManager.GetBestMove(_algorythmPlayer);
            ExecutePlayerRequest(() => _gameManager.PlayMove(_algorythmPlayer, column));
        }
        private void ExecutePlayerRequest(Action action)
        {
            try
            {
                action();
            }
            catch (InvalidPlayerRequestException e)
            {
                Logger.Log(LogLevel.Error, LogContext.ALGORYTHM_PLAYER, "Invalid player request from algorythm player.", e);
            }
        }

        protected override void OnDispose()
        {
            _gameManager.OnRequestedMatch -= OnRequestedMatch;
            _gameManager.OnGameStarted -= OnGameStarted;
            _gameManager.OnConfirmedGameStart -= OnConfirmedGameStart;
            _gameManager.OnMovePlayed -= OnMovePlayed;
        }

        private readonly GameManager _gameManager;
        private bool _algorythmPlayerIsStartingPlayer;
        private Player? _opponentPlayer;
        private AlgorythmPlayer? _algorythmPlayer;
    }
}

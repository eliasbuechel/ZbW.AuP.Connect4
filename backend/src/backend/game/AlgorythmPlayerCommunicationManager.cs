using backend.communication.signalR.frontendApi;
using backend.Infrastructure;
using backend.services.player;
using backend.services;
using backend.game.entities;

namespace backend.game
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

            int column = _gameManager.GetBestMove(_algorythmPlayer);
            _gameManager.PlayMove(_algorythmPlayer, column);
        }

        private void OnRequestedMatch(Player requester, Player opponent)
        {
            if (opponent is AlgorythmPlayer opponentAlgorythmPlayer)
                _gameManager.AcceptMatch(opponentAlgorythmPlayer, requester);
        }
        private void OnGameStarted(Game game)
        {
            if (game.Match.Player1 is AlgorythmPlayer algorythmPlayer1)
            {
                if (game.Match.Player1 == game.ActivePlayer)
                {
                    _algorythmPlayerIsStartingPlayer = true;
                    _opponentPlayer = game.Match.Player2;
                }

                _algorythmPlayer = algorythmPlayer1;
                _gameManager.ConfirmGameStart(algorythmPlayer1);
            }

            if (game.Match.Player2 is AlgorythmPlayer algorythmPlayer2)
            {
                if (game.Match.Player2 == game.ActivePlayer)
                {
                    _algorythmPlayerIsStartingPlayer = true;
                    _opponentPlayer = game.Match.Player1;
                }

                _algorythmPlayer = algorythmPlayer2;
                _gameManager.ConfirmGameStart(algorythmPlayer2);
            }
        }
        private void OnConfirmedGameStart(Player player)
        {
            if (_opponentPlayer == null || _algorythmPlayer == null)
                return;

            if (player != _opponentPlayer)
                return;

            if (!_algorythmPlayerIsStartingPlayer)
                return;

            _algorythmPlayerIsStartingPlayer = false;
            int column = _gameManager.GetBestMove(_algorythmPlayer);
            _gameManager.PlayMove(_algorythmPlayer, column);
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

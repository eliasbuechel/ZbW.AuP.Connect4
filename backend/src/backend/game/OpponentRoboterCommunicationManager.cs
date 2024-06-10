using backend.communication.DOTs;
using backend.communication.signalR.opponentRoboterApi;
using backend.game.entities;
using backend.Infrastructure;
using backend.services;
using backend.services.player;
using System.ComponentModel;

namespace backend.game
{
    internal class OpponentRoboterCommunicationManager : DisposingObject
    {
        public OpponentRoboterCommunicationManager(
            OpponentRoboterHubApi opponentRoboterHubApi,
            OpponentRoboterClientApi opponentRoboterClientApi,
            PlayerConnectionService playerConnectionService,
            GameManager gameManager
            )
        {
            _opponentRoboterHubApi = opponentRoboterHubApi;
            _opponentRoboterClientApi = opponentRoboterClientApi;
            _playerConnectionService = playerConnectionService;
            _gameManager = gameManager;

            _opponentRoboterHubApi.OnRequestMatch += OnRequestMatch;
            _opponentRoboterHubApi.OnAcceptMatch += OnAcceptMatch;
            _opponentRoboterHubApi.OnRejectMatch += OnRejectMatch;
            _opponentRoboterHubApi.OnConfirmGameStart += OnConfirmGameStart;
            _opponentRoboterHubApi.OnPlayMove += OnPlayMove;
            _opponentRoboterHubApi.OnQuitGame += OnQuitGame;

            _opponentRoboterClientApi.OnRequestMatch += OnRequestMatch;
            _opponentRoboterClientApi.OnAcceptMatch += OnAcceptMatch;
            _opponentRoboterClientApi.OnRejectMatch += OnRejectMatch;
            _opponentRoboterClientApi.OnConfirmGameStart += OnConfirmGameStart;
            _opponentRoboterClientApi.OnPlayMove += OnPlayMove;
            _opponentRoboterClientApi.OnQuitGame += OnQuitGame;

            _gameManager.OnRequestedMatch += OnRequestedMatch;
            _gameManager.OnMatched += OnMatched;
            _gameManager.OnRejectedMatch += OnRejectedMatch;
            _gameManager.OnConfirmedGameStart += OnConfirmedGameStart;
            _gameManager.OnMovePlayed += OnMovePlayed;
            _gameManager.OnGameEnded += OnGameEnded;
        }

        // reciving
        private void OnRequestMatch(string identification)
        {
            OpponentRoboterPlayer opponentRoboterPlayer = _playerConnectionService.OpponentRoboterPlayerConnectionManager.GetConnectedPlayerByIdentification(identification);
            _playerConnectionService.AlgorythmPlayerConnectionManager.ConnectPlayer(opponentRoboterPlayer, identification);
            AlgorythmPlayer algorythmPlayer = _playerConnectionService.AlgorythmPlayerConnectionManager.GetConnectedPlayerByIdentification(opponentRoboterPlayer);

            _gameManager.RequestMatch(opponentRoboterPlayer, algorythmPlayer);
        }
        public void OnAcceptMatch(string identification)
        {
            OpponentRoboterPlayer opponentRoboterPlayer = _playerConnectionService.OpponentRoboterPlayerConnectionManager.GetConnectedPlayerByIdentification(identification);
            AlgorythmPlayer algorythmPlayer = _playerConnectionService.AlgorythmPlayerConnectionManager.GetConnectedPlayerByIdentification(opponentRoboterPlayer);

            _gameManager.AcceptMatch(opponentRoboterPlayer, algorythmPlayer);
        }
        public void OnRejectMatch(string identification)
        {
            OpponentRoboterPlayer opponentRoboterPlayer = _playerConnectionService.OpponentRoboterPlayerConnectionManager.GetConnectedPlayerByIdentification(identification);
            AlgorythmPlayer algorythmPlayer = _playerConnectionService.AlgorythmPlayerConnectionManager.GetConnectedPlayerByIdentification(opponentRoboterPlayer);

            _gameManager.RejectMatch(opponentRoboterPlayer, algorythmPlayer);
        }
        public void OnConfirmGameStart(string identification)
        {
            OpponentRoboterPlayer opponentRoboterPlayer = _playerConnectionService.OpponentRoboterPlayerConnectionManager.GetConnectedPlayerByIdentification(identification);
            _gameManager.ConfirmGameStart(opponentRoboterPlayer);
        }
        public void OnPlayMove(string identification, int column)
        {
            OpponentRoboterPlayer opponentRoboterPlayer = _playerConnectionService.OpponentRoboterPlayerConnectionManager.GetConnectedPlayerByIdentification(identification);
            _gameManager.PlayMove(opponentRoboterPlayer, column);
        }
        public void OnQuitGame(string identification)
        {
            OpponentRoboterPlayer opponentRoboterPlayer = _playerConnectionService.OpponentRoboterPlayerConnectionManager.GetConnectedPlayerByIdentification(identification);
            _gameManager.QuitGame(opponentRoboterPlayer);
        }

        // sending
        private void OnRequestedMatch(Player requester, Player opponent)
        {
            if (opponent is OpponentRoboterPlayer opponentRboboterPlayer)
            {
                if (opponentRboboterPlayer.IsHubPlayer)
                    _playerConnectionService.OpponentRoboterPlayerConnectionManager.ForeachConnectionOfPlayer(opponentRboboterPlayer, c => _opponentRoboterHubApi.Send_RequestMatch(c));
                else
                    _opponentRoboterClientApi.Send_RequestMatch();
            }
        }
        private void OnRejectedMatch(Player rejecter, Player opponent)
        {
            if (opponent is OpponentRoboterPlayer opponentRboboterPlayer)
            {
                if (opponentRboboterPlayer.IsHubPlayer)
                    _playerConnectionService.OpponentRoboterPlayerConnectionManager.ForeachConnectionOfPlayer(opponentRboboterPlayer, c => _opponentRoboterHubApi.Send_RejectMatch(c));
                else
                    _opponentRoboterClientApi.Send_RejectMatch();
            }
        }
        private void OnMatched(Match match)
        {
            if (match.Player1 is OpponentRoboterPlayer opponentRboboterPlayer1)
            {
                if (opponentRboboterPlayer1.IsHubPlayer)
                    _playerConnectionService.OpponentRoboterPlayerConnectionManager.ForeachConnectionOfPlayer(opponentRboboterPlayer1, c => _opponentRoboterHubApi.Send_AcceptMatch(c));
                else
                    _opponentRoboterClientApi.Send_AcceptMatch();
            }

            if (match.Player2 is OpponentRoboterPlayer opponentRboboterPlayer2)
            {
                if (opponentRboboterPlayer2.IsHubPlayer)
                    _playerConnectionService.OpponentRoboterPlayerConnectionManager.ForeachConnectionOfPlayer(opponentRboboterPlayer2, c => _opponentRoboterHubApi.Send_AcceptMatch(c));
                else
                    _opponentRoboterClientApi.Send_AcceptMatch();
            }
        }
        private void OnGameEnded(GameResult gameResult)
        {
            if (gameResult.WinnerId != null && gameResult.Line == null)
            {
                Player player1 = _playerConnectionService.GetPlayer(gameResult.Match.Player1.Id);
                Player player2 = _playerConnectionService.GetPlayer(gameResult.Match.Player2.Id);

                if (player2.Id == gameResult.WinnerId && player1 is OpponentRoboterPlayer opponentRboboterPlayer1)
                {
                    if (opponentRboboterPlayer1.IsHubPlayer)
                        _playerConnectionService.OpponentRoboterPlayerConnectionManager.ForeachConnectionOfPlayer(opponentRboboterPlayer1, c => _opponentRoboterHubApi.Send_QuitGame(c));
                    else
                        _opponentRoboterClientApi.Send_QuitGame();
                }

                if (player1.Id == gameResult.WinnerId && player2 is OpponentRoboterPlayer opponentRboboterPlayer2)
                {
                    if (opponentRboboterPlayer2.IsHubPlayer)
                        _playerConnectionService.OpponentRoboterPlayerConnectionManager.ForeachConnectionOfPlayer(opponentRboboterPlayer2, c => _opponentRoboterHubApi.Send_QuitGame(c));
                    else
                        _opponentRoboterClientApi.Send_QuitGame();
                }
            }
        }
        private void OnConfirmedGameStart(Player player)
        {
            if (player is AlgorythmPlayer algorythmPlayer)
            {
                if (algorythmPlayer.OpponentPlayer is OpponentRoboterPlayer opponentRboboterPlayer)
                {
                    if (opponentRboboterPlayer.IsHubPlayer)
                        _playerConnectionService.OpponentRoboterPlayerConnectionManager.ForeachConnectionOfPlayer(opponentRboboterPlayer, c => _opponentRoboterHubApi.Send_ConfirmGameStart(c));
                    else
                        _opponentRoboterClientApi.Send_ConfirmGameStart();
                }
            }
        }
        private void OnMovePlayed(Player player, Field field)
        {
            if (player is AlgorythmPlayer algorythmPlayer)
            {
                if (algorythmPlayer.OpponentPlayer is OpponentRoboterPlayer opponentRboboterPlayer)
                {
                    if (opponentRboboterPlayer.IsHubPlayer)
                        _playerConnectionService.OpponentRoboterPlayerConnectionManager.ForeachConnectionOfPlayer(opponentRboboterPlayer, c => _opponentRoboterHubApi.Send_PlayMove(c, field.Column));
                    else
                        _opponentRoboterClientApi.Send_PlayMove(field.Column);
                }
            }
        }

        protected override void OnDispose()
        {
            _opponentRoboterHubApi.OnRequestMatch -= OnRequestMatch;
            _opponentRoboterHubApi.OnAcceptMatch -= OnAcceptMatch;
            _opponentRoboterHubApi.OnRejectMatch -= OnRejectMatch;
            _opponentRoboterHubApi.OnConfirmGameStart -= OnConfirmGameStart;
            _opponentRoboterHubApi.OnPlayMove -= OnPlayMove;
            _opponentRoboterHubApi.OnQuitGame -= OnQuitGame;

            _opponentRoboterClientApi.OnRequestMatch -= OnRequestMatch;
            _opponentRoboterClientApi.OnAcceptMatch -= OnAcceptMatch;
            _opponentRoboterClientApi.OnRejectMatch -= OnRejectMatch;
            _opponentRoboterClientApi.OnConfirmGameStart -= OnConfirmGameStart;
            _opponentRoboterClientApi.OnPlayMove -= OnPlayMove;
            _opponentRoboterClientApi.OnQuitGame -= OnQuitGame;

            _gameManager.OnRequestedMatch -= OnRequestedMatch;
            _gameManager.OnMatched -= OnMatched;
            _gameManager.OnRejectedMatch -= OnRejectedMatch;
            _gameManager.OnConfirmedGameStart -= OnConfirmedGameStart;
            _gameManager.OnMovePlayed -= OnMovePlayed;
            _gameManager.OnGameEnded -= OnGameEnded;
        }

        private readonly OpponentRoboterHubApi _opponentRoboterHubApi;
        private readonly OpponentRoboterClientApi _opponentRoboterClientApi;
        private readonly PlayerConnectionService _playerConnectionService;
        private readonly GameManager _gameManager;
    }
}

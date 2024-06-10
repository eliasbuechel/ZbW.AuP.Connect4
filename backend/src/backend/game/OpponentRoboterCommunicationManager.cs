using backend.communication.signalR.opponentRoboterApi;
using backend.Infrastructure;
using backend.services;
using backend.services.player;

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
        }

        private void OnRequestMatch(string identification)
        {
            OpponentRoboterPlayer opponentRoboterPlayer = _playerConnectionService.OpponentRoboterPlayerConnectionManager.GetConnectedPlayer(identification);
            _playerConnectionService.AlgorythmPlayerConnectionManager.ConnectPlayer(opponentRoboterPlayer, identification);
            AlgorythmPlayer algorythmPlayer = _playerConnectionService.AlgorythmPlayerConnectionManager.GetConnectedPlayer(opponentRoboterPlayer);

            _gameManager.RequestMatch(opponentRoboterPlayer, algorythmPlayer);
        }
        public void OnAcceptMatch(string identification)
        {
            OpponentRoboterPlayer opponentRoboterPlayer = _playerConnectionService.OpponentRoboterPlayerConnectionManager.GetConnectedPlayer(identification);
            AlgorythmPlayer algorythmPlayer = _playerConnectionService.AlgorythmPlayerConnectionManager.GetConnectedPlayer(opponentRoboterPlayer);

            _gameManager.AcceptMatch(opponentRoboterPlayer, algorythmPlayer);
        }
        public void OnRejectMatch(string identification)
        {
            OpponentRoboterPlayer opponentRoboterPlayer = _playerConnectionService.OpponentRoboterPlayerConnectionManager.GetConnectedPlayer(identification);
            AlgorythmPlayer algorythmPlayer = _playerConnectionService.AlgorythmPlayerConnectionManager.GetConnectedPlayer(opponentRoboterPlayer);

            _gameManager.RejectMatch(opponentRoboterPlayer, algorythmPlayer);
        }
        public void OnConfirmGameStart(string identification)
        {
            OpponentRoboterPlayer opponentRoboterPlayer = _playerConnectionService.OpponentRoboterPlayerConnectionManager.GetConnectedPlayer(identification);
            _gameManager.ConfirmGameStart(opponentRoboterPlayer);
        }
        public void OnPlayMove(string identification, int column)
        {
            OpponentRoboterPlayer opponentRoboterPlayer = _playerConnectionService.OpponentRoboterPlayerConnectionManager.GetConnectedPlayer(identification);
            _gameManager.PlayMove(opponentRoboterPlayer, column);
        }
        public void OnQuitGame(string identification)
        {
            OpponentRoboterPlayer opponentRoboterPlayer = _playerConnectionService.OpponentRoboterPlayerConnectionManager.GetConnectedPlayer(identification);
            _gameManager.QuitGame(opponentRoboterPlayer);
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
        }

        private readonly OpponentRoboterHubApi _opponentRoboterHubApi;
        private readonly OpponentRoboterClientApi _opponentRoboterClientApi;
        private readonly PlayerConnectionService _playerConnectionService;
        private readonly GameManager _gameManager;
    }
}

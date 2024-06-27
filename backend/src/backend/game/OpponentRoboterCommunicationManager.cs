using backend.communication.signalR.opponentRoboterApi;
using backend.game.entities;
using backend.infrastructure;
using backend.services;
using backend.services.player;

namespace backend.game
{
    internal class OpponentRoboterCommunicationManager : DisposingObject
    {
        public OpponentRoboterCommunicationManager(
            OpponentRoboterHubApi opponentRoboterHubApi,
            OpponentRoboterClientApiManager opponentRoboterClientApiManager,
            PlayerConnectionService playerConnectionService,
            GameManager gameManager
            )
        {
            _opponentRoboterHubApi = opponentRoboterHubApi;
            _opponentRoboterClientApiManager = opponentRoboterClientApiManager;
            _playerConnectionService = playerConnectionService;
            _gameManager = gameManager;

            _opponentRoboterHubApi.OnRequestMatch += OnRequestMatch;
            _opponentRoboterHubApi.OnAcceptMatch += OnAcceptMatch;
            _opponentRoboterHubApi.OnRejectMatch += OnRejectMatch;
            _opponentRoboterHubApi.OnConfirmGameStart += OnConfirmGameStart;
            _opponentRoboterHubApi.OnPlayMove += OnPlayMove;
            _opponentRoboterHubApi.OnQuitGame += OnQuitGame;

            _opponentRoboterClientApiManager.OnCreated += OnCreatedOpponentRoboterClientApi;
            _opponentRoboterClientApiManager.ForEach((opponentRoboterClientApi) =>
            {
                opponentRoboterClientApi.OnRequestMatch += OnRequestMatch;
                opponentRoboterClientApi.OnAcceptMatch += OnAcceptMatch;
                opponentRoboterClientApi.OnRejectMatch += OnRejectMatch;
                opponentRoboterClientApi.OnConfirmGameStart += OnConfirmGameStart;
                opponentRoboterClientApi.OnPlayMove += OnPlayMove;
                opponentRoboterClientApi.OnQuitGame += OnQuitGame;
            });

            _gameManager.OnRequestedMatch += OnRequestedMatch;
            _gameManager.OnMatched += OnMatched;
            _gameManager.OnRejectedMatch += OnRejectedMatch;
            _gameManager.OnConfirmedGameStart += OnConfirmedGameStart;
            _gameManager.OnMovePlayed += OnMovePlayed;
            _gameManager.OnGameEnded += OnGameEnded;
        }

        private void OnCreatedOpponentRoboterClientApi(OpponentRoboterClientApi opponentRoboterClientApi)
        {
            opponentRoboterClientApi.OnRequestMatch += OnRequestMatch;
            opponentRoboterClientApi.OnAcceptMatch += OnAcceptMatch;
            opponentRoboterClientApi.OnRejectMatch += OnRejectMatch;
            opponentRoboterClientApi.OnConfirmGameStart += OnConfirmGameStart;
            opponentRoboterClientApi.OnPlayMove += OnPlayMove;
            opponentRoboterClientApi.OnQuitGame += OnQuitGame;
        }

        // reciving
        private void OnRequestMatch(string connectionId)
        {
            OpponentRoboterPlayer opponentRoboterPlayer = _playerConnectionService.OpponentRoboterPlayerConnectionManager.GetConnectedPlayerByConnectionId(connectionId);
            _playerConnectionService.AlgorythmPlayerConnectionManager.ConnectPlayer(opponentRoboterPlayer, connectionId);
            AlgorythmPlayer algorythmPlayer = _playerConnectionService.AlgorythmPlayerConnectionManager.GetConnectedPlayerByIdentification(opponentRoboterPlayer);

            _gameManager.RequestMatch(opponentRoboterPlayer, algorythmPlayer);
        }
        public void OnAcceptMatch(string connectionId)
        {
            OpponentRoboterPlayer opponentRoboterPlayer = _playerConnectionService.OpponentRoboterPlayerConnectionManager.GetConnectedPlayerByConnectionId(connectionId);
            AlgorythmPlayer algorythmPlayer = _playerConnectionService.AlgorythmPlayerConnectionManager.GetConnectedPlayerByIdentification(opponentRoboterPlayer);

            _gameManager.AcceptMatch(opponentRoboterPlayer, algorythmPlayer);
        }
        public void OnRejectMatch(string connectionId)
        {
            OpponentRoboterPlayer opponentRoboterPlayer = _playerConnectionService.OpponentRoboterPlayerConnectionManager.GetConnectedPlayerByConnectionId(connectionId);
            AlgorythmPlayer algorythmPlayer = _playerConnectionService.AlgorythmPlayerConnectionManager.GetConnectedPlayerByIdentification(opponentRoboterPlayer);

            _gameManager.RejectMatch(opponentRoboterPlayer, algorythmPlayer);
        }
        public void OnConfirmGameStart(string connectionId)
        {
            OpponentRoboterPlayer opponentRoboterPlayer = _playerConnectionService.OpponentRoboterPlayerConnectionManager.GetConnectedPlayerByConnectionId(connectionId);
            _gameManager.ConfirmGameStart(opponentRoboterPlayer);
        }
        public void OnPlayMove(string connectionId, int column)
        {
            OpponentRoboterPlayer opponentRoboterPlayer = _playerConnectionService.OpponentRoboterPlayerConnectionManager.GetConnectedPlayerByConnectionId(connectionId);
            _gameManager.PlayMove(opponentRoboterPlayer, column);
        }
        public void OnQuitGame(string connectionId)
        {
            OpponentRoboterPlayer opponentRoboterPlayer = _playerConnectionService.OpponentRoboterPlayerConnectionManager.GetConnectedPlayerByConnectionId(connectionId);
            _gameManager.QuitGame(opponentRoboterPlayer);
        }

        // sending
        private async void OnRequestedMatch(Player requester, Player opponent)
        {
            if (opponent is OpponentRoboterPlayer opponentRboboterPlayer)
            {
                if (opponentRboboterPlayer.IsHubPlayer)
                    _playerConnectionService.OpponentRoboterPlayerConnectionManager.ForeachConnectionOfPlayer(opponentRboboterPlayer, async c => await _opponentRoboterHubApi.Send_RequestMatch(c));
                else
                {
                    OpponentRoboterClientApi opponentRoboterClientApi = _opponentRoboterClientApiManager.Get(opponentRboboterPlayer.Identification);
                    await opponentRoboterClientApi.Send_RequestMatch();
                }
            }
        }
        private async void OnRejectedMatch(Player rejecter, Player opponent)
        {
            if (opponent is OpponentRoboterPlayer opponentRboboterPlayer)
            {
                if (opponentRboboterPlayer.IsHubPlayer)
                    _playerConnectionService.OpponentRoboterPlayerConnectionManager.ForeachConnectionOfPlayer(opponentRboboterPlayer, async c => await _opponentRoboterHubApi.Send_RejectMatch(c));
                else
                {
                    OpponentRoboterClientApi opponentRoboterClientApi = _opponentRoboterClientApiManager.Get(opponentRboboterPlayer.Identification);
                    await opponentRoboterClientApi.Send_RejectMatch();

                }
            }
        }
        private async void OnMatched(Match match)
        {
            if (match.Player2 is OpponentRoboterPlayer opponentRboboterPlayer2)
            {
                if (opponentRboboterPlayer2.IsHubPlayer)
                    _playerConnectionService.OpponentRoboterPlayerConnectionManager.ForeachConnectionOfPlayer(opponentRboboterPlayer2, async c => await _opponentRoboterHubApi.Send_AcceptMatch(c));
                else
                {
                    OpponentRoboterClientApi opponentRoboterClientApi = _opponentRoboterClientApiManager.Get(opponentRboboterPlayer2.Identification);
                    await opponentRoboterClientApi.Send_AcceptMatch();
                }
            }
        }
        private void OnGameEnded(GameResult gameResult)
        {
            if (gameResult.WinnerId != null && gameResult.Line == null)
            {
                Player? player1 = _playerConnectionService.GetPlayerOrDefault(gameResult.Match.Player1.Id);
                Player? player2 = _playerConnectionService.GetPlayerOrDefault(gameResult.Match.Player2.Id);

                if (player1 != null && gameResult.Match.Player2.Id == gameResult.WinnerId && player1 is OpponentRoboterPlayer opponentRboboterPlayer1)
                    OnQuitGame(opponentRboboterPlayer1);
                else if (player2 != null && gameResult.Match.Player1.Id == gameResult.WinnerId && player2 is OpponentRoboterPlayer opponentRboboterPlayer2)
                    OnQuitGame(opponentRboboterPlayer2);
            }
        }
        private async void OnQuitGame(OpponentRoboterPlayer opponentRoboterPlayer)
        {
            if (opponentRoboterPlayer.IsHubPlayer)
                _playerConnectionService.OpponentRoboterPlayerConnectionManager.ForeachConnectionOfPlayer(opponentRoboterPlayer, async c => await _opponentRoboterHubApi.Send_QuitGame(c));
            else
            {
                OpponentRoboterClientApi opponentRoboterClientApi = _opponentRoboterClientApiManager.Get(opponentRoboterPlayer.Identification);
                await opponentRoboterClientApi.Send_QuitGame();
            }
        }
        private async void OnConfirmedGameStart(Player player)
        {
            if (player is AlgorythmPlayer algorythmPlayer && algorythmPlayer.OpponentPlayer is OpponentRoboterPlayer opponentRboboterPlayer)
            {
                if (opponentRboboterPlayer.IsHubPlayer)
                    _playerConnectionService.OpponentRoboterPlayerConnectionManager.ForeachConnectionOfPlayer(opponentRboboterPlayer, async c => await _opponentRoboterHubApi.Send_ConfirmGameStart(c));
                else
                {
                    OpponentRoboterClientApi opponentRoboterClientApi = _opponentRoboterClientApiManager.Get(opponentRboboterPlayer.Identification);
                    await opponentRoboterClientApi.Send_ConfirmGameStart();
                }
            }
        }
        private async void OnMovePlayed(Player player, Field field)
        {
            Logger.Log(LogLevel.Debug, LogContext.GAME_PLAY, $"Invoke playing move in opponentRoboterCommunicationManager. Player: {player.Username} Column: {field.Column}");

            if (player is AlgorythmPlayer algorythmPlayer && algorythmPlayer.OpponentPlayer is OpponentRoboterPlayer opponentRboboterPlayer)
            {
                if (opponentRboboterPlayer.IsHubPlayer)
                    _playerConnectionService.OpponentRoboterPlayerConnectionManager.ForeachConnectionOfPlayer(opponentRboboterPlayer, async c => await _opponentRoboterHubApi.Send_PlayMove(c, field.Column));
                else
                {
                    OpponentRoboterClientApi opponentRoboterClientApi = _opponentRoboterClientApiManager.Get(opponentRboboterPlayer.Identification);
                    await opponentRoboterClientApi.Send_PlayMove(field.Column);
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

            _opponentRoboterClientApiManager.OnCreated -= OnCreatedOpponentRoboterClientApi;
            _opponentRoboterClientApiManager.ForEach((opponentRoboterClientApi) =>
            {
                opponentRoboterClientApi.OnRequestMatch -= OnRequestMatch;
                opponentRoboterClientApi.OnAcceptMatch -= OnAcceptMatch;
                opponentRoboterClientApi.OnRejectMatch -= OnRejectMatch;
                opponentRoboterClientApi.OnConfirmGameStart -= OnConfirmGameStart;
                opponentRoboterClientApi.OnPlayMove -= OnPlayMove;
                opponentRoboterClientApi.OnQuitGame -= OnQuitGame;
            });

            _gameManager.OnRequestedMatch -= OnRequestedMatch;
            _gameManager.OnMatched -= OnMatched;
            _gameManager.OnRejectedMatch -= OnRejectedMatch;
            _gameManager.OnConfirmedGameStart -= OnConfirmedGameStart;
            _gameManager.OnMovePlayed -= OnMovePlayed;
            _gameManager.OnGameEnded -= OnGameEnded;
        }

        private readonly OpponentRoboterHubApi _opponentRoboterHubApi;
        private readonly OpponentRoboterClientApiManager _opponentRoboterClientApiManager;
        private readonly PlayerConnectionService _playerConnectionService;
        private readonly GameManager _gameManager;
    }
}

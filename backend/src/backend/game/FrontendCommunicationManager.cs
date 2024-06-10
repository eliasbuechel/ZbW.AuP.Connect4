using backend.communication.DOTs;
using backend.communication.signalR.frontendApi;
using backend.Data;
using backend.game.entities;
using backend.Infrastructure;
using backend.services;
using backend.services.player;
using System.Diagnostics;

namespace backend.game
{
    internal delegate void RequestMatch(Player requester, Player opponent);
    internal delegate void AcceptMatch(Player accepter, Player opponent);
    internal delegate void RejectMatch(Player rejecter, Player opponent);
    internal delegate void ConfirmedGameStart(Player player);
    internal delegate void PlayMove(Player player, int column);
    internal delegate void QuitGame(Player player);

    internal delegate void WatchGame(Player player);
    internal delegate void StopWatchingGame(Player player);

    internal class FrontendCommunicationManager : DisposingObject
    {
        public FrontendCommunicationManager(FrontendApi frontendApi, PlayerConnectionService playerConnectionService, GameManager gameManager)
        {
            _frontendApi = frontendApi;
            _playerConnectionService = playerConnectionService;
            _gameManager = gameManager;

            _playerConnectionService.WebPlayerConnectionManager.OnPlayerDisconnected += PlayerDisconnected;
            _frontendApi.OnGetUserData += GetUserData;
            _frontendApi.OnRequestMatch += RequestMatch;
            _frontendApi.OnAcceptMatch += AcceptMatch;
            _frontendApi.OnRejectMatch += RejectMatch;
            _frontendApi.OnConfirmedGameStart += ConfirmedGameStart;
            _frontendApi.OnPlayMove += PlayMove;
            _frontendApi.OnQuitGame += QuitGame;

            _frontendApi.OnWatchGame += WatchGame;
            _frontendApi.OnStopWatchingGame += StopWatchingGame;

            _frontendApi.OnRequestSinglePlayerMatch += RequestSinglePlayerMatch;
            _frontendApi.OnRequestMatchFromOpponentRoboterPlayer += RequestMatchFromOpponentRoboterPlayer;

            _gameManager.OnRequestedMatch += OnRequestedMatch;
            _gameManager.OnRejectedMatch += OnRejectedMatch;
            _gameManager.OnMatched += OnMatched;
            _gameManager.OnGameStarted += OnGameStarted;
            _gameManager.OnGameEnded += OnGameEnded;
            _gameManager.OnConfirmedGameStart += OnConfirmedGameStart;
            _gameManager.OnMovePlayed += OnMovePlayed;
            _gameManager.OnSendHint += OnSendHint;
        }

        public event RequestMatch? OnRequestMatch;
        public event AcceptMatch? OnAcceptMatch;
        public event RejectMatch? OnRejectMatch;
        public event ConfirmedGameStart? OnConfirmGameStart;
        public event PlayMove? OnPlayMove;
        public event QuitGame? OnQuitGame;

        public event WatchGame? OnWatchGame;
        public event StopWatchingGame? OnStopWatchingGame;

        protected override void OnDispose()
        {
            _frontendApi.OnGetUserData -= GetUserData;
            _frontendApi.OnRequestMatch -= RequestMatch;
            _frontendApi.OnAcceptMatch -= AcceptMatch;
            _frontendApi.OnRejectMatch -= RejectMatch;
            _frontendApi.OnConfirmedGameStart -= ConfirmedGameStart;
            _frontendApi.OnPlayMove -= PlayMove;
            _frontendApi.OnQuitGame -= QuitGame;

            _frontendApi.OnWatchGame -= WatchGame;
            _frontendApi.OnStopWatchingGame -= StopWatchingGame;

            _frontendApi.OnRequestSinglePlayerMatch -= RequestSinglePlayerMatch;
            _frontendApi.OnRequestMatchFromOpponentRoboterPlayer -= RequestMatchFromOpponentRoboterPlayer;

            _gameManager.OnRequestedMatch -= OnRequestedMatch;
            _gameManager.OnRejectedMatch -= OnRejectedMatch;
            _gameManager.OnMatched -= OnMatched;
            _gameManager.OnGameStarted -= OnGameStarted;
            _gameManager.OnGameEnded -= OnGameEnded;
            _gameManager.OnConfirmedGameStart -= OnConfirmedGameStart;
            _gameManager.OnMovePlayed -= OnMovePlayed;
        }

        // requests
        private void GetUserData(PlayerIdentity playerIdentity, string connectionId)
        {
            WebPlayer webPlayer = _playerConnectionService.WebPlayerConnectionManager.GetConnectedPlayer(playerIdentity);
            SendUserData(connectionId, webPlayer);
        }
        private void RequestMatch(PlayerIdentity requestingPlayerIdentity, string opponentPlayerId)
        {
            WebPlayer requestingWebPlayer = _playerConnectionService.WebPlayerConnectionManager.GetConnectedPlayer(requestingPlayerIdentity);
            Player? opponentPlayer = _playerConnectionService.GetPlayer(opponentPlayerId);
            if (opponentPlayer == null)
            {
                Debug.Assert(false);
                return;
            }

            _gameManager.RequestMatch(requestingWebPlayer, opponentPlayer);
        }
        private void AcceptMatch(PlayerIdentity acceptingPlayerIdentity, string opponentPlayerId)
        {
            WebPlayer acceptingWebPlayer = _playerConnectionService.WebPlayerConnectionManager.GetConnectedPlayer(acceptingPlayerIdentity);
            Player opponentPlayer = _playerConnectionService.GetPlayer(opponentPlayerId);

            _gameManager.AcceptMatch(acceptingWebPlayer, opponentPlayer);
        }
        private void RejectMatch(PlayerIdentity rejectingPlayerIdentity, string opponentPlayerId)
        {
            WebPlayer rejectingWebPlayer = _playerConnectionService.WebPlayerConnectionManager.GetConnectedPlayer(rejectingPlayerIdentity);
            Player opponentPlayer = _playerConnectionService.GetPlayer(opponentPlayerId);

            _gameManager.RejectMatch(rejectingWebPlayer, opponentPlayer);
        }
        private void ConfirmedGameStart(PlayerIdentity playerIdentity)
        {
            WebPlayer webPlayer = _playerConnectionService.WebPlayerConnectionManager.GetConnectedPlayer(playerIdentity);
            _gameManager.ConfirmGameStart(webPlayer);
        }
        private void PlayMove(PlayerIdentity playerIdentity, int column)
        {
            WebPlayer webPlayer = _playerConnectionService.WebPlayerConnectionManager.GetConnectedPlayer(playerIdentity);
            _gameManager.PlayMove(webPlayer, column);
        }
        private void QuitGame(PlayerIdentity playerIdentity)
        {
            WebPlayer webPlayer = _playerConnectionService.WebPlayerConnectionManager.GetConnectedPlayer(playerIdentity);
            _gameManager.QuitGame(webPlayer);
        }
        private void PlayerDisconnected(WebPlayer webPlayer)
        {
            _gameManager.PlayerDisconnected(webPlayer);

            foreach (var p in _playerConnectionService.Players)
            {
                if (p.MatchingRequests.Contains(webPlayer))
                    p.MatchingRequests.Remove(webPlayer);

                if (p.Matching == webPlayer)
                    p.Matching = null;
            }
        }

        private void WatchGame(PlayerIdentity playerIdentity)
        {
            WebPlayer webPlayer = _playerConnectionService.WebPlayerConnectionManager.GetConnectedPlayer(playerIdentity);
            webPlayer.WatchingGame = true;
        }
        private void StopWatchingGame(PlayerIdentity playerIdentity)
        {
            WebPlayer webPlayer = _playerConnectionService.WebPlayerConnectionManager.GetConnectedPlayer(playerIdentity);
            webPlayer.WatchingGame = false;
        }

        private void RequestMatchFromOpponentRoboterPlayer(string opponentRoboterPlayerId)
        {
            OpponentRoboterPlayer opponentRoboterPlayer = _playerConnectionService.OpponentRoboterPlayerConnectionManager.GetConnectedPlayer(opponentRoboterPlayerId);
            _playerConnectionService.AlgorythmPlayerConnectionManager.ConnectPlayer(opponentRoboterPlayer, "Algorythm player");
            AlgorythmPlayer algorythmPlayer = _playerConnectionService.AlgorythmPlayerConnectionManager.GetConnectedPlayer(opponentRoboterPlayer);

            _gameManager.RequestMatch(opponentRoboterPlayer, algorythmPlayer);
        }
        private void RequestSinglePlayerMatch(PlayerIdentity requestingPlayerIdentity)
        {
            WebPlayer requestingPlayer = _playerConnectionService.WebPlayerConnectionManager.GetConnectedPlayer(requestingPlayerIdentity);
            _playerConnectionService.AlgorythmPlayerConnectionManager.ConnectPlayer(requestingPlayer, "Algorythm player");
            AlgorythmPlayer algorythmPlayer = _playerConnectionService.AlgorythmPlayerConnectionManager.GetConnectedPlayer(requestingPlayer);

            _gameManager.RequestMatch(requestingPlayer, algorythmPlayer);
        }


        // responses
        private async void SendUserData(string connectionId, Player player)
        {
            PlayerInfoDTO playerInfoDTO = new PlayerInfoDTO(player);
            await _frontendApi.SendUserData(connectionId, playerInfoDTO);
        }
        private void OnRequestedMatch(Player requester, Player opponent)
        {
            if (opponent is WebPlayer opponentWebPlayer)
                _playerConnectionService.WebPlayerConnectionManager.ForeachConnectionOfPlayer(opponentWebPlayer, async c => await _frontendApi.PlayerRequestedMatch(c, requester.Id));
        }
        private void OnRejectedMatch(Player rejecter, Player opponent)
        {
            if (opponent is WebPlayer opponentWebPlayer)
                _playerConnectionService.WebPlayerConnectionManager.ForeachConnectionOfPlayer(opponentWebPlayer, async c => await _frontendApi.PlayerRejectedMatch(c, rejecter.Id));
        }
        private void OnMatched(Match match)
        {
            MatchDTO matchDTO = new MatchDTO(match);
            _playerConnectionService.WebPlayerConnectionManager.ForeachConnectedPlayerConnection(async c => await _frontendApi.Matched(c, matchDTO));
        }
        private void OnGameStarted(Game game)
        {
            GameDTO gameDTO = new GameDTO(game);
            _playerConnectionService.WebPlayerConnectionManager.ForeachConnectedPlayerConnection(async c => await _frontendApi.GameStarted(c, gameDTO));
        }
        private void OnGameEnded(GameResult gameResult)
        {
            GameResultDTO gameResultDTO = new GameResultDTO(gameResult);
            _playerConnectionService.WebPlayerConnectionManager.ForeachConnectedPlayerConnection(async c => await _frontendApi.GameEnded(c, gameResultDTO));
        }
        private void OnConfirmedGameStart(Player player)
        {
            _playerConnectionService.WebPlayerConnectionManager.ForeachConnectedPlayerConnection(async c => await _frontendApi.ConfirmedGameStart(c, player.Id));
        }
        private void OnMovePlayed(Player player, Field field)
        {
            FieldDTO fieldDTO = new FieldDTO(field);
            _playerConnectionService.WebPlayerConnectionManager.ForeachConnectedPlayerConnection(async c => await _frontendApi.MovePlayed(c, player.Id, fieldDTO));
        }
        private void OnSendHint(Player player, int column)
        {
            if (player is WebPlayer webPlayer)
                _playerConnectionService.WebPlayerConnectionManager.ForeachConnectionOfPlayer(webPlayer, async c => await _frontendApi.SendHint(c, column));
        }


        private readonly FrontendApi _frontendApi;
        private readonly PlayerConnectionService _playerConnectionService;
        private readonly GameManager _gameManager;
    }
}

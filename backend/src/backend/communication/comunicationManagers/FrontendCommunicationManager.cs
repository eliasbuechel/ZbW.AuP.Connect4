using backend.communication.dtos;
using backend.communication.mqtt;
using backend.communication.signalR.frontendApi;
using backend.communication.signalR.opponentRoboterApi;
using backend.data;
using backend.game;
using backend.game.entities;
using backend.game.players;
using backend.infrastructure;
using backend.services;
using System.Diagnostics;

namespace backend.communication.comunicationManagers
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
        public FrontendCommunicationManager(
            FrontendApi frontendApi,
            PlayerConnectionService playerConnectionService,
            GameManager gameManager,
            GameResultsService gameResultsService,
            OpponentRoboterClientApiManager opponentRoboterClientApiManager,
            RoboterApi roboterApi,
            Board board
            )
        {
            _frontendApi = frontendApi;
            _playerConnectionService = playerConnectionService;
            _gameManager = gameManager;
            _gameResultsService = gameResultsService;
            _opponentRoboterClientApiManager = opponentRoboterClientApiManager;
            _roboterApi = roboterApi;
            _board = board;

            _playerConnectionService.WebPlayerConnectionManager.OnPlayerDisconnected += PlayerDisconnected;

            _frontendApi.OnGetConnectedPlayers += GetConnectedPlayers;
            _frontendApi.OnGetGamePlan += GetGamePlan;
            _frontendApi.OnGetGame += GetGame;
            _frontendApi.OnGetBestlist += GetBestlist;
            _frontendApi.OnGetHint += GetHint;
            _frontendApi.OnGetVisualisationState += GetVisualisationState;

            _frontendApi.OnGetUserData += GetUserData;
            _frontendApi.OnRequestMatch += RequestMatch;
            _frontendApi.OnAcceptMatch += AcceptMatch;
            _frontendApi.OnRejectMatch += RejectMatch;
            _frontendApi.OnConfirmedGameStart += ConfirmedGameStart;
            _frontendApi.OnPlayMove += PlayMove;
            _frontendApi.OnQuitGame += QuitGame;

            _frontendApi.OnWatchGame += WatchGame;
            _frontendApi.OnStopWatchingGame += StopWatchingGame;

            _frontendApi.OnConnectToOpponentRoboterPlayer += OnConnectToOpponentRoboterPlayer;
            _frontendApi.OnRequestSinglePlayerMatch += RequestSinglePlayerMatch;
            _frontendApi.OnRequestOppoenntRoboterPlyerMatch += RequestOpponentRoboterPlayerMatch;
            _frontendApi.OnAcceptOppoenntRoboterPlyerMatch += AcceptOppoenntRoboterPlyerMatch;
            _frontendApi.OnRejectOppoenntRoboterPlyerMatch += RejectOppoenntRoboterPlyerMatch;

            _frontendApi.OnVisualizeOnRoboterChanged += VisualizeOnRoboterChanged;

            _gameManager.OnRequestedMatch += OnRequestedMatch;
            _gameManager.OnRejectedMatch += OnRejectedMatch;
            _gameManager.OnMatched += OnMatched;
            _gameManager.OnGameStarted += OnGameStarted;
            _gameManager.OnGameEnded += OnGameEnded;
            _gameManager.OnConfirmedGameStart += OnConfirmedGameStart;
            _gameManager.OnMovePlayed += OnMovePlayed;
            _gameManager.OnSendHint += OnSendHint;
            _gameManager.OnPlacingStone += OnPlacingStone;
        }

        protected override void OnDispose()
        {
            _playerConnectionService.WebPlayerConnectionManager.OnPlayerDisconnected -= PlayerDisconnected;

            _frontendApi.OnGetConnectedPlayers -= GetConnectedPlayers;
            _frontendApi.OnGetGamePlan -= GetGamePlan;
            _frontendApi.OnGetGame -= GetGame;
            _frontendApi.OnGetBestlist -= GetBestlist;
            _frontendApi.OnGetHint -= GetHint;
            _frontendApi.OnGetVisualisationState -= GetVisualisationState;

            _frontendApi.OnGetUserData -= GetUserData;
            _frontendApi.OnRequestMatch -= RequestMatch;
            _frontendApi.OnAcceptMatch -= AcceptMatch;
            _frontendApi.OnRejectMatch -= RejectMatch;
            _frontendApi.OnConfirmedGameStart -= ConfirmedGameStart;
            _frontendApi.OnPlayMove -= PlayMove;
            _frontendApi.OnQuitGame -= QuitGame;

            _frontendApi.OnWatchGame -= WatchGame;
            _frontendApi.OnStopWatchingGame -= StopWatchingGame;

            _frontendApi.OnConnectToOpponentRoboterPlayer -= OnConnectToOpponentRoboterPlayer;
            _frontendApi.OnRequestSinglePlayerMatch -= RequestSinglePlayerMatch;
            _frontendApi.OnRequestOppoenntRoboterPlyerMatch -= RequestOpponentRoboterPlayerMatch;
            _frontendApi.OnAcceptOppoenntRoboterPlyerMatch -= AcceptOppoenntRoboterPlyerMatch;
            _frontendApi.OnRejectOppoenntRoboterPlyerMatch -= RejectOppoenntRoboterPlyerMatch;

            _frontendApi.OnVisualizeOnRoboterChanged -= VisualizeOnRoboterChanged;

            _gameManager.OnRequestedMatch -= OnRequestedMatch;
            _gameManager.OnRejectedMatch -= OnRejectedMatch;
            _gameManager.OnMatched -= OnMatched;
            _gameManager.OnGameStarted -= OnGameStarted;
            _gameManager.OnGameEnded -= OnGameEnded;
            _gameManager.OnConfirmedGameStart -= OnConfirmedGameStart;
            _gameManager.OnMovePlayed -= OnMovePlayed;
        }

        // requests
        private async void GetUserData(PlayerIdentity playerIdentity, string connectionId)
        {
            WebPlayer webPlayer = _playerConnectionService.WebPlayerConnectionManager.GetConnectedPlayerByIdentification(playerIdentity);
            await SendUserData(connectionId, webPlayer);
        }
        public async void GetConnectedPlayers(PlayerIdentity playerIdentity, string connectionId)
        {
            WebPlayer webPlayer = _playerConnectionService.WebPlayerConnectionManager.GetConnectedPlayerByIdentification(playerIdentity);
            ConnectedPlayersDto connectedPlayers = _playerConnectionService.GetConnectedPlayersExcept(webPlayer);

            await SendConnectedPlayers(connectionId, connectedPlayers);
        }
        public async void GetGamePlan(string connectionId)
        {
            IEnumerable<Match> gamePlan = _gameManager.GamePlan;
            await SendGamePlan(connectionId, gamePlan);
        }

        public async void GetGame(PlayerIdentity playerIdentity, string connectionId)
        {
            WebPlayer webPlayer = _playerConnectionService.WebPlayerConnectionManager.GetConnectedPlayerByIdentification(playerIdentity);

            Game? game = _gameManager.Game;
            if (game == null)
                return;

            if (!webPlayer.IsInGame && !webPlayer.IsWatchingGame)
                return;

            await SendGame(connectionId, game);
        }

        public async void GetBestlist(string connectionId)
        {
            IEnumerable<GameResult> bestlist = _gameResultsService.Bestlist;
            await SendBestlist(connectionId, bestlist);
        }
        public void GetHint(PlayerIdentity playerIdentity)
        {
            WebPlayer webPlayer = _playerConnectionService.WebPlayerConnectionManager.GetConnectedPlayerByIdentification(playerIdentity);
            _gameManager.GetHint(webPlayer);
        }
        private void GetVisualisationState(string connectionId)
        {
            SendVisualisationState(_board.IsVisualizingOnRoboter, connectionId);
        }

        private void RequestMatch(PlayerIdentity requestingPlayerIdentity, string opponentPlayerId)
        {

            WebPlayer requestingWebPlayer = _playerConnectionService.WebPlayerConnectionManager.GetConnectedPlayerByIdentification(requestingPlayerIdentity);
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
            WebPlayer acceptingWebPlayer = _playerConnectionService.WebPlayerConnectionManager.GetConnectedPlayerByIdentification(acceptingPlayerIdentity);
            Player opponentPlayer = _playerConnectionService.GetPlayer(opponentPlayerId);

            _gameManager.AcceptMatch(acceptingWebPlayer, opponentPlayer);
        }
        private void RejectMatch(PlayerIdentity rejectingPlayerIdentity, string opponentPlayerId)
        {
            WebPlayer rejectingWebPlayer = _playerConnectionService.WebPlayerConnectionManager.GetConnectedPlayerByIdentification(rejectingPlayerIdentity);
            Player opponentPlayer = _playerConnectionService.GetPlayer(opponentPlayerId);

            _gameManager.RejectMatch(rejectingWebPlayer, opponentPlayer);
        }
        private void ConfirmedGameStart(PlayerIdentity playerIdentity)
        {
            WebPlayer webPlayer = _playerConnectionService.WebPlayerConnectionManager.GetConnectedPlayerByIdentification(playerIdentity);
            _gameManager.ConfirmGameStart(webPlayer);
        }
        private void PlayMove(PlayerIdentity playerIdentity, int column)
        {
            WebPlayer webPlayer = _playerConnectionService.WebPlayerConnectionManager.GetConnectedPlayerByIdentification(playerIdentity);
            _gameManager.PlayMove(webPlayer, column);
        }
        private void QuitGame(PlayerIdentity playerIdentity)
        {
            WebPlayer webPlayer = _playerConnectionService.WebPlayerConnectionManager.GetConnectedPlayerByIdentification(playerIdentity);
            _gameManager.QuitGame(webPlayer);
        }
        private void PlayerDisconnected(WebPlayer webPlayer)
        {
            _gameManager.PlayerDisconnected(webPlayer);

            foreach (var p in _playerConnectionService.Players)
            {
                MatchRequest? matchRequest = p.MatchingRequests.FirstOrDefault(x => x.Player.Equals(webPlayer));
                if (matchRequest != null)
                    p.MatchingRequests.Remove(matchRequest);

                if (p.Matching == webPlayer)
                    p.Matching = null;
            }
        }

        private void WatchGame(PlayerIdentity playerIdentity)
        {
            WebPlayer webPlayer = _playerConnectionService.WebPlayerConnectionManager.GetConnectedPlayerByIdentification(playerIdentity);
            webPlayer.IsWatchingGame = true;

            Game? game = _gameManager.Game;
            if (game == null)
                return;

            _playerConnectionService.WebPlayerConnectionManager.ForeachConnectionOfPlayer(webPlayer, async c => await SendGame(c, game));
        }
        private void StopWatchingGame(PlayerIdentity playerIdentity)
        {
            WebPlayer webPlayer = _playerConnectionService.WebPlayerConnectionManager.GetConnectedPlayerByIdentification(playerIdentity);
            webPlayer.IsWatchingGame = false;
            OnYouStoppedWatchingGame(webPlayer);
        }

        private void OnConnectToOpponentRoboterPlayer(string hubUrl, string connectionId)
        {
            try
            {
                _opponentRoboterClientApiManager.Create(hubUrl);
            }
            catch (UriFormatException e)
            {
                string errorMessage = "Not able to connect! " + e.Message;
                OnNotAbleToConnectToOpponentRoboterPlayer(connectionId, errorMessage);
            }
        }
        private void RequestOpponentRoboterPlayerMatch(string requestingOpponentRoboterPlayerId)
        {
            OpponentRoboterPlayer opponentRoboterPlayer = _playerConnectionService.OpponentRoboterPlayerConnectionManager.GetConnectedPlayer(requestingOpponentRoboterPlayerId);
            _playerConnectionService.AlgorythmPlayerConnectionManager.ConnectPlayer(opponentRoboterPlayer, "R4D4-Algorythm");
            AlgorythmPlayer algorythmPlayer = _playerConnectionService.AlgorythmPlayerConnectionManager.GetConnectedPlayerByIdentification(opponentRoboterPlayer);

            _gameManager.RequestMatch(algorythmPlayer, opponentRoboterPlayer);
        }
        private void AcceptOppoenntRoboterPlyerMatch(string acceptingOpponentRoboterPlayerId)
        {
            OpponentRoboterPlayer opponentRoboterPlayer = _playerConnectionService.OpponentRoboterPlayerConnectionManager.GetConnectedPlayer(acceptingOpponentRoboterPlayerId);
            AlgorythmPlayer algorythmPlayer = _playerConnectionService.AlgorythmPlayerConnectionManager.GetConnectedPlayerByIdentification(opponentRoboterPlayer);

            _gameManager.AcceptMatch(algorythmPlayer, opponentRoboterPlayer);
        }
        private void RejectOppoenntRoboterPlyerMatch(string rejectingOpponentRoboterPlayerId)
        {
            OpponentRoboterPlayer opponentRoboterPlayer = _playerConnectionService.OpponentRoboterPlayerConnectionManager.GetConnectedPlayer(rejectingOpponentRoboterPlayerId);
            AlgorythmPlayer algorythmPlayer = _playerConnectionService.AlgorythmPlayerConnectionManager.GetConnectedPlayerByIdentification(opponentRoboterPlayer);

            _gameManager.RejectMatch(algorythmPlayer, opponentRoboterPlayer);
        }

        private void RequestSinglePlayerMatch(PlayerIdentity requestingPlayerIdentity)
        {
            WebPlayer requestingPlayer = _playerConnectionService.WebPlayerConnectionManager.GetConnectedPlayerByIdentification(requestingPlayerIdentity);
            _playerConnectionService.AlgorythmPlayerConnectionManager.ConnectPlayer(requestingPlayer, "R4D4-Algorythm");
            AlgorythmPlayer algorythmPlayer = _playerConnectionService.AlgorythmPlayerConnectionManager.GetConnectedPlayerByIdentification(requestingPlayer);

            _gameManager.RequestMatch(requestingPlayer, algorythmPlayer);
        }

        private void VisualizeOnRoboterChanged(bool isVisualizingOnRoboter)
        {
            _playerConnectionService.WebPlayerConnectionManager.ForeachConnectedPlayerConnection(async (c) => await _frontendApi.SendVisualisationState(isVisualizingOnRoboter, c));
            _roboterApi.IsVisualizingOnRoboter = isVisualizingOnRoboter;
        }



        // responses
        private async Task SendUserData(string connectionId, Player player)
        {
            PlayerInfoDto playerInfoDTO = new(player);
            await _frontendApi.SendUserData(connectionId, playerInfoDTO);
        }
        private async Task SendConnectedPlayers(string connectionId, ConnectedPlayersDto connectedPlayersDTO)
        {
            await _frontendApi.SendConnectedPlayers(connectionId, connectedPlayersDTO);
        }
        private async Task SendBestlist(string connectionId, IEnumerable<GameResult> bestlist)
        {
            IEnumerable<GameResultDto> bestlistDTO = bestlist.Select(x => new GameResultDto(x));
            await _frontendApi.SendBestList(connectionId, bestlistDTO);
        }
        private async Task SendGame(string connectionId, Game game)
        {
            GameDto gameDTO = new(game);
            await _frontendApi.SendGame(connectionId, gameDTO);
        }
        private async Task SendGamePlan(string connectionId, IEnumerable<Match> gamePlan)
        {
            IEnumerable<MatchDto> gamePlanDTO = gamePlan.Select(x => new MatchDto(x));
            await _frontendApi.SendGamePlan(connectionId, gamePlanDTO);
        }
        private async void SendVisualisationState(bool isVisualizingOnRoboter, string connectionId)
        {
            await _frontendApi.SendVisualisationState(isVisualizingOnRoboter, connectionId);
        }

        private void OnRequestedMatch(Player requester, Player opponent)
        {
            if (opponent is WebPlayer opponentWebPlayer)
                _playerConnectionService.WebPlayerConnectionManager.ForeachConnectionOfPlayer(opponentWebPlayer, async c => await _frontendApi.PlayerRequestedMatch(c, requester.Id));
            if (requester is WebPlayer requesterWebPlayer)
                _playerConnectionService.WebPlayerConnectionManager.ForeachConnectionOfPlayer(requesterWebPlayer, async c => await _frontendApi.YouRequestedMatch(c, opponent.Id));
            if (opponent is OpponentRoboterPlayer)
                _playerConnectionService.WebPlayerConnectionManager.ForeachConnectedPlayerConnection(async c => await _frontendApi.YouRequestedMatch(c, opponent.Id));
            if (requester is OpponentRoboterPlayer)
                _playerConnectionService.WebPlayerConnectionManager.ForeachConnectedPlayerConnection(async c => await _frontendApi.PlayerRequestedMatch(c, requester.Id));
        }
        private void OnRejectedMatch(Player rejecter, Player opponent)
        {
            if (opponent is WebPlayer opponentWebPlayer)
                _playerConnectionService.WebPlayerConnectionManager.ForeachConnectionOfPlayer(opponentWebPlayer, async c => await _frontendApi.PlayerRejectedMatch(c, rejecter.Id));
            if (rejecter is WebPlayer rejecterWebPlayer)
                _playerConnectionService.WebPlayerConnectionManager.ForeachConnectionOfPlayer(rejecterWebPlayer, async c => await _frontendApi.YouRejectedMatch(c, opponent.Id));
            if (opponent is OpponentRoboterPlayer)
                _playerConnectionService.WebPlayerConnectionManager.ForeachConnectedPlayerConnection(async c => await _frontendApi.YouRejectedMatch(c, opponent.Id));
            if (rejecter is OpponentRoboterPlayer)
                _playerConnectionService.WebPlayerConnectionManager.ForeachConnectedPlayerConnection(async c => await _frontendApi.PlayerRejectedMatch(c, rejecter.Id));
        }
        private void OnMatched(Match match)
        {
            MatchDto matchDTO = new(match);
            _playerConnectionService.WebPlayerConnectionManager.ForeachConnectedPlayerConnection(async c => await _frontendApi.Matched(c, matchDTO));
        }
        private void OnGameStarted(Game game)
        {
            GameDto gameDTO = new(game);
            _playerConnectionService.WebPlayerConnectionManager.ForeachConnectedPlayerConnection(_sendGameInformationCondition, async c => await _frontendApi.GameStarted(c, gameDTO));
        }
        private void OnGameEnded(GameResult gameResult)
        {
            GameResultDto gameResultDTO = new(gameResult);

            _playerConnectionService.WebPlayerConnectionManager.ForeachConnectedPlayerConnection(async c => await _frontendApi.GameEnded(c, gameResultDTO));

            if (!_gameManager.GamePlan.Any())
                foreach (var p in _playerConnectionService.WebPlayerConnectionManager.ConnectedPlayers)
                    p.IsWatchingGame = false;

            _playerConnectionService.WebPlayerConnectionManager.ForeachConnectedPlayerConnection(c => GetBestlist(c));
        }
        private void OnConfirmedGameStart(Player player)
        {
            _playerConnectionService.WebPlayerConnectionManager.ForeachConnectedPlayerConnection(_sendGameInformationCondition, async c => await _frontendApi.ConfirmedGameStart(c, player.Id));
        }
        private void OnMovePlayed(Player player, Field field)
        {
            FieldDto fieldDTO = new(field);
            _playerConnectionService.WebPlayerConnectionManager.ForeachConnectedPlayerConnection(_sendGameInformationCondition, async c => await _frontendApi.MovePlayed(c, player.Id, fieldDTO));
        }
        private void OnPlacingStone(Player player, Field field)
        {
            FieldDto fieldDTO = new(field);
            _playerConnectionService.WebPlayerConnectionManager.ForeachConnectedPlayerConnection(_sendGameInformationCondition, async c => await _frontendApi.PlacingStone(c, player.Id, fieldDTO));
        }
        private void OnSendHint(Player player, int column)
        {
            if (player is WebPlayer webPlayer)
                _playerConnectionService.WebPlayerConnectionManager.ForeachConnectionOfPlayer(webPlayer, async c => await _frontendApi.SendHint(c, column));
        }

        private void OnYouStoppedWatchingGame(WebPlayer webPlayer)
        {
            _playerConnectionService.WebPlayerConnectionManager.ForeachConnectionOfPlayer(webPlayer, async c => await _frontendApi.YouStoppedWatchingGame(c));
        }
        private async void OnNotAbleToConnectToOpponentRoboterPlayer(string connectionId, string errorMessage)
        {
            await _frontendApi.NotAbleToConnectToOpponentRoboterPlayer(connectionId, errorMessage);
        }


        private readonly FrontendApi _frontendApi;
        private readonly PlayerConnectionService _playerConnectionService;
        private readonly GameManager _gameManager;
        private readonly GameResultsService _gameResultsService;
        private readonly OpponentRoboterClientApiManager _opponentRoboterClientApiManager;
        private readonly RoboterApi _roboterApi;
        private readonly Board _board;
        private readonly Func<WebPlayer, bool> _sendGameInformationCondition = (p) => p.IsInGame || p.IsWatchingGame;
    }
}

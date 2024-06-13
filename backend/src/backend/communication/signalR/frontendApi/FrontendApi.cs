using backend.communication.DOTs;
using backend.Data;
using backend.game;
using backend.utilities;
using Microsoft.AspNetCore.SignalR;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace backend.communication.signalR.frontendApi
{
    internal delegate void GetUserData(PlayerIdentity playerIdentity, string connectionId);
    internal delegate void GetConnectedPlayers(PlayerIdentity playerIdentity, string connectionId);
    internal delegate void GetGamePlan(string connectionId);
    internal delegate void GetGame(PlayerIdentity playerIdentity, string connectionId);
    internal delegate void GetBestlist(string connectionId);
    internal delegate void GetHint(PlayerIdentity playerIdentity);

    internal delegate void RequestMatch(PlayerIdentity requestingPlayerIdentity, string opponentPlayerId);
    internal delegate void AcceptMatch(PlayerIdentity acceptingPlayerIdentity, string opponentPlayerId);
    internal delegate void RejectMatch(PlayerIdentity rejectingPlayerIdentity, string opponentPlayerId);
    internal delegate void ConfirmedGameStart(PlayerIdentity rejectingPlayerIdentity);
    internal delegate void PlayMove(PlayerIdentity rejectingPlayerIdentity, int column);
    internal delegate void QuitGame(PlayerIdentity rejectingPlayerIdentity);

    internal delegate void WatchGame(PlayerIdentity playerIdentity);
    internal delegate void StopWatchingGame(PlayerIdentity playerIdentity);

    internal delegate void RequestSinglePlayerMatch(PlayerIdentity requestingPlayerIdentity);
    internal delegate void ConnectToOpponentRoboterPlayer(string hubUrl);
    internal delegate void RequestMatchFromOpponentRoboterPlayer(string requestingOpponentRoboterPlayerId);
    internal delegate void AcceptOppoenntRoboterPlyerMatch(string acceptingOpponentRoboterPlayerId);
    internal delegate void RejectOppoenntRoboterPlyerMatch(string rejectingOpponentRoboterPlayerId);

    internal class FrontendApi : SignalRApi<FrontendHub, PlayerIdentity>
    {
        public FrontendApi(
            IHubContext<FrontendHub> hubClient,
            RequestHandlerManager<PlayerIdentity> requestHandlerManager
            ) : base(hubClient, requestHandlerManager)
        { }

        public event GetUserData? OnGetUserData;
        public event GetConnectedPlayers? OnGetConnectedPlayers;
        public event GetGamePlan? OnGetGamePlan;
        public event GetGame? OnGetGame;
        public event GetBestlist? OnGetBestlist;
        public event GetHint? OnGetHint;

        public event RequestMatch? OnRequestMatch;
        public event AcceptMatch? OnAcceptMatch;
        public event RejectMatch? OnRejectMatch;
        public event ConfirmedGameStart? OnConfirmedGameStart;
        public event PlayMove? OnPlayMove;
        public event QuitGame? OnQuitGame;

        public event WatchGame? OnWatchGame;
        public event StopWatchingGame? OnStopWatchingGame;

        public event RequestSinglePlayerMatch? OnRequestSinglePlayerMatch;
        public event ConnectToOpponentRoboterPlayer? OnConnectToOpponentRoboterPlayer;
        public event RequestMatchFromOpponentRoboterPlayer? OnRequestOppoenntRoboterPlyerMatch;
        public event AcceptOppoenntRoboterPlyerMatch? OnAcceptOppoenntRoboterPlyerMatch;
        public event RejectOppoenntRoboterPlyerMatch? OnRejectOppoenntRoboterPlyerMatch;

        // reciving
        public void GetUserData(PlayerIdentity playerIdentity, string connectionId)
        {
            _requestHandlerManager.GetOrCreateHandler(playerIdentity).Enqueue(() =>
            {
                OnGetUserData?.Invoke(playerIdentity, connectionId);
                return Task.CompletedTask;
            });
        }
        public void GetConnectedPlayers(PlayerIdentity playerIdentity, string connectionId)
        {
            _requestHandlerManager.GetOrCreateHandler(playerIdentity).Enqueue(() =>
            {
                OnGetConnectedPlayers?.Invoke(playerIdentity, connectionId);
                return Task.CompletedTask;
            });
        }
        public void GetGamePlan(PlayerIdentity playerIdentity, string connectionId)
        {
            _requestHandlerManager.GetOrCreateHandler(playerIdentity).Enqueue(() =>
            {
                OnGetGamePlan?.Invoke(connectionId);
                return Task.CompletedTask;
            });
        }
        public void GetGame(PlayerIdentity playerIdentity, string connectionId)
        {
            _requestHandlerManager.GetOrCreateHandler(playerIdentity).Enqueue(() =>
            {
                OnGetGame?.Invoke(playerIdentity, connectionId);
                return Task.CompletedTask;
            });
        }
        public void GetBestlist(PlayerIdentity playerIdentity, string connectionId)
        {
            _requestHandlerManager.GetOrCreateHandler(playerIdentity).Enqueue(() =>
            {
                OnGetBestlist?.Invoke(connectionId);
                return Task.CompletedTask;
            });
        }
        public void GetHint(PlayerIdentity playerIdentity)
        {
            _requestHandlerManager.GetOrCreateHandler(playerIdentity).Enqueue(() =>
            {
                OnGetHint?.Invoke(playerIdentity);
                return Task.CompletedTask;
            });
        }

        public void RequestMatch(PlayerIdentity requestingPlayerIdentity, string opponentPlayerId)
        {
            _requestHandlerManager.GetOrCreateHandler(requestingPlayerIdentity).Enqueue(() =>
            {
                OnRequestMatch?.Invoke(requestingPlayerIdentity, opponentPlayerId);
                return Task.CompletedTask;
            });
        }
        public void AcceptMatch(PlayerIdentity acceptingPlayerIdentity, string opponentPlayerId)
        {
            _requestHandlerManager.GetOrCreateHandler(acceptingPlayerIdentity).Enqueue(() =>
            {
                OnAcceptMatch?.Invoke(acceptingPlayerIdentity, opponentPlayerId);
                return Task.CompletedTask;
            });
        }
        public void RejectMatch(PlayerIdentity rejectingPlayerIdentity, string opponentPlayerId)
        {
            _requestHandlerManager.GetOrCreateHandler(rejectingPlayerIdentity).Enqueue(() =>
            {
                OnRejectMatch?.Invoke(rejectingPlayerIdentity, opponentPlayerId);
                return Task.CompletedTask;
            });
        }
        public void ConfirmGameStart(PlayerIdentity playerIdentity)
        {
            _requestHandlerManager.GetOrCreateHandler(playerIdentity).Enqueue(() =>
            {
                OnConfirmedGameStart?.Invoke(playerIdentity);
                return Task.CompletedTask;
            });
        }
        public void PlayMove(PlayerIdentity playerIdentity, int column)
        {
            _requestHandlerManager.GetOrCreateHandler(playerIdentity).Enqueue(() =>
            {
                OnPlayMove?.Invoke(playerIdentity, column);
                return Task.CompletedTask;
            });
        }
        public void QuitGame(PlayerIdentity playerIdentity)
        {
            _requestHandlerManager.GetOrCreateHandler(playerIdentity).Enqueue(() =>
            {
                OnQuitGame?.Invoke(playerIdentity);
                return Task.CompletedTask;
            });
        }

        public void WatchGame(PlayerIdentity playerIdentity)
        {
            _requestHandlerManager.GetOrCreateHandler(playerIdentity).Enqueue(() =>
            {
                OnWatchGame?.Invoke(playerIdentity);
                return Task.CompletedTask;
            });
        }
        public void StopWatchingGame(PlayerIdentity playerIdentity)
        {
            _requestHandlerManager.GetOrCreateHandler(playerIdentity).Enqueue(() =>
            {
                OnStopWatchingGame?.Invoke(playerIdentity);
                return Task.CompletedTask;
            });
        }

        public void RequestSinglePlayerMatch(PlayerIdentity requestingPlayerIdentity)
        {
            _requestHandlerManager.GetOrCreateHandler(requestingPlayerIdentity).Enqueue(() =>
            {
                OnRequestSinglePlayerMatch?.Invoke(requestingPlayerIdentity);
                return Task.CompletedTask;
            });
        }
        public void RequestOppoenntRoboterPlyerMatch(PlayerIdentity playerIdentity, string requestingOpponentRoboterPlayerId)
        {
            _requestHandlerManager.GetOrCreateHandler(playerIdentity).Enqueue(() =>
            {
                OnRequestOppoenntRoboterPlyerMatch?.Invoke(requestingOpponentRoboterPlayerId);
                return Task.CompletedTask;
            });
        }
        public void AcceptOppoenntRoboterPlyerMatch(PlayerIdentity playerIdentity, string acceptingOpponentRoboterPlayerId)
        {
            _requestHandlerManager.GetOrCreateHandler(playerIdentity).Enqueue(() =>
            {
                OnAcceptOppoenntRoboterPlyerMatch?.Invoke(acceptingOpponentRoboterPlayerId);
                return Task.CompletedTask;
            });
        }
        public void RejectOppoenntRoboterPlyerMatch(PlayerIdentity playerIdentity, string rejectingOpponentRoboterPlayerId)
        {
            _requestHandlerManager.GetOrCreateHandler(playerIdentity).Enqueue(() =>
            {
                OnRejectOppoenntRoboterPlyerMatch?.Invoke(rejectingOpponentRoboterPlayerId);
                return Task.CompletedTask;
            });
        }
        public void ConnectToOpponentRoboterPlayer(PlayerIdentity initiatorPlayerIdentity, string hubUrl)
        {
            _requestHandlerManager.GetOrCreateHandler(initiatorPlayerIdentity).Enqueue(() =>
            {
                OnConnectToOpponentRoboterPlayer?.Invoke(hubUrl);
                return Task.CompletedTask;
            });
        }


        // sending
        public async Task SendUserData(string connection, PlayerInfoDTO userData)
        {
            await _hubConetext.Clients.Client(connection).SendAsync(nameof(SendUserData), userData);
        }
        public async Task SendConnectedPlayers(string connection, ConnectedPlayersDTO connectedPlayers)
        {
            await _hubConetext.Clients.Client(connection).SendAsync(nameof(SendConnectedPlayers), connectedPlayers);
        }
        public async Task SendGamePlan(string connection, IEnumerable<MatchDTO> gamePlan)
        {
            await _hubConetext.Clients.Client(connection).SendAsync(nameof(SendGamePlan), gamePlan);
        }
        public async Task SendGame(string connection, GameDTO game)
        {
            await _hubConetext.Clients.Client(connection).SendAsync(nameof(SendGame), game);
        }
        public async Task SendBestList(string connection, IEnumerable<GameResultDTO> bestlist)
        {
            await _hubConetext.Clients.Client(connection).SendAsync(nameof(SendBestList), bestlist);
        }
        public async Task SendHint(string connection, int column)
        {
            await _hubConetext.Clients.Client(connection).SendAsync(nameof(SendHint), column);
        }

        public async Task PlayerConnected(string connection, ConnectedPlayerDTO onlinePlayer)
        {
            await _hubConetext.Clients.Client(connection).SendAsync(nameof(PlayerConnected), onlinePlayer);
        }
        public async Task PlayerDisconnected(string connection, string playerId)
        {
            await _hubConetext.Clients.Client(connection).SendAsync(nameof(PlayerDisconnected), playerId);
        }
        public async Task OpponentRoboterPlayerConnected(string connection, ConnectedPlayerDTO opponentRoboterPlayer)
        {
            await _hubConetext.Clients.Client(connection).SendAsync(nameof(OpponentRoboterPlayerConnected), opponentRoboterPlayer);
        }
        public async Task OpponentRoboterPlayerDisconnected(string connection, string playerId)
        {
            await _hubConetext.Clients.Client(connection).SendAsync(nameof(OpponentRoboterPlayerDisconnected), playerId);
        }

        public async Task PlayerRequestedMatch(string connection, string playerId)
        {
            await _hubConetext.Clients.Client(connection).SendAsync(nameof(PlayerRequestedMatch), playerId);
        }
        public async Task YouRequestedMatch(string connection, string playerId)
        {
            await _hubConetext.Clients.Client(connection).SendAsync(nameof(YouRequestedMatch), playerId);
        }
        public async Task PlayerRejectedMatch(string connection, string playerId)
        {
            await _hubConetext.Clients.Client(connection).SendAsync(nameof(PlayerRejectedMatch), playerId);
        }
        public async Task YouRejectedMatch(string connection, string playerId)
        {
            await _hubConetext.Clients.Client(connection).SendAsync(nameof(YouRejectedMatch), playerId);
        }
        public async Task Matched(string connection, MatchDTO match)
        {
            await _hubConetext.Clients.Client(connection).SendAsync(nameof(Matched), match);
        }
        public async Task GameStarted(string connection, GameDTO connect4Game)
        {
            await _hubConetext.Clients.Client(connection).SendAsync(nameof(GameStarted), connect4Game);
        }
        public async Task GameEnded(string connection, GameResultDTO gameResult)
        {
            await _hubConetext.Clients.Client(connection).SendAsync(nameof(GameEnded), gameResult);
        }
        public async Task ConfirmedGameStart(string connection, string playerId)
        {
            await _hubConetext.Clients.Client(connection).SendAsync(nameof(ConfirmedGameStart), playerId);
        }
        public async Task MovePlayed(string connection, string playerId, FieldDTO field)
        {
            await _hubConetext.Clients.Client(connection).SendAsync(nameof(MovePlayed), playerId, field);
        }
        public async Task YouStoppedWatchingGame(string connection)
        {
            await _hubConetext.Clients.Client(connection).SendAsync(nameof(YouStoppedWatchingGame));
        }
    }
}
using backend.communication.DOTs;
using backend.Data;
using backend.utilities;
using Microsoft.AspNetCore.SignalR;

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
    internal delegate void ConnectToOpponentRoboterPlayer(string hubUrl, string connectionId);
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
            Func<Task> methode = () =>
            {
                OnGetUserData?.Invoke(playerIdentity, connectionId);
                return Task.CompletedTask;
            };

            Request(playerIdentity, methode, connectionId);
        }
        public void GetConnectedPlayers(PlayerIdentity playerIdentity, string connectionId)
        {
            Func<Task> methode = () =>
            {
                OnGetConnectedPlayers?.Invoke(playerIdentity, connectionId);
                return Task.CompletedTask;
            };

            Request(playerIdentity, methode, connectionId);
        }
        public void GetGamePlan(PlayerIdentity playerIdentity, string connectionId)
        {
            Func<Task> methode = () =>
            {
                OnGetGamePlan?.Invoke(connectionId);
                return Task.CompletedTask;
            };

            Request(playerIdentity, methode, connectionId);
        }
        public void GetGame(PlayerIdentity playerIdentity, string connectionId)
        {
            Func<Task> methode = () =>
            {
                OnGetGame?.Invoke(playerIdentity, connectionId);
                return Task.CompletedTask;
            };

            Request(playerIdentity, methode, connectionId);
        }
        public void GetBestlist(PlayerIdentity playerIdentity, string connectionId)
        {
            Func<Task> methode = () =>
            {
                OnGetBestlist?.Invoke(connectionId);
                return Task.CompletedTask;
            };

            Request(playerIdentity, methode, connectionId);
        }
        public void GetHint(PlayerIdentity playerIdentity, string connectionId)
        {
            Func<Task> methode = () =>
            {
                OnGetHint?.Invoke(playerIdentity);
                return Task.CompletedTask;
            };

            Request(playerIdentity, methode, connectionId);
        }

        public void RequestMatch(PlayerIdentity requestingPlayerIdentity, string opponentPlayerId, string connectionId)
        {
            Func<Task> methode = () =>
            {
                OnRequestMatch?.Invoke(requestingPlayerIdentity, opponentPlayerId);
                return Task.CompletedTask;
            };

            Request(requestingPlayerIdentity, methode, connectionId);
        }
        public void AcceptMatch(PlayerIdentity acceptingPlayerIdentity, string opponentPlayerId, string connectionId)
        {
            Func<Task> methode = () =>
            {
                OnAcceptMatch?.Invoke(acceptingPlayerIdentity, opponentPlayerId);
                return Task.CompletedTask;
            };

            Request(acceptingPlayerIdentity, methode, connectionId);
        }
        public void RejectMatch(PlayerIdentity rejectingPlayerIdentity, string opponentPlayerId, string connectionId)
        {
            Func<Task> methode = () =>
            {
                OnRejectMatch?.Invoke(rejectingPlayerIdentity, opponentPlayerId);
                return Task.CompletedTask;
            };

            Request(rejectingPlayerIdentity, methode, connectionId);
        }
        public void ConfirmGameStart(PlayerIdentity playerIdentity, string connectionId)
        {
            Func<Task> methode = () =>
            {
                OnConfirmedGameStart?.Invoke(playerIdentity);
                return Task.CompletedTask;
            };

            Request(playerIdentity, methode, connectionId);
        }
        public void PlayMove(PlayerIdentity playerIdentity, int column, string connectionId)
        {
            Func<Task> methode = () =>
            {
                OnPlayMove?.Invoke(playerIdentity, column);
                return Task.CompletedTask;
            };

            Request(playerIdentity, methode, connectionId);
        }
        public void QuitGame(PlayerIdentity playerIdentity, string connectionId)
        {
            Func<Task> methode = () =>
            {
                OnQuitGame?.Invoke(playerIdentity);
                return Task.CompletedTask;
            };

            Request(playerIdentity, methode, connectionId);
        }

        public void WatchGame(PlayerIdentity playerIdentity, string connectionId)
        {
            Func<Task> methode = () =>
            {
                OnWatchGame?.Invoke(playerIdentity);
                return Task.CompletedTask;
            };

            Request(playerIdentity, methode, connectionId);
        }
        public void StopWatchingGame(PlayerIdentity playerIdentity, string connectionId)
        {
            Func<Task> methode = () =>
            {
                OnStopWatchingGame?.Invoke(playerIdentity);
                return Task.CompletedTask;
            };

            Request(playerIdentity, methode, connectionId);
        }

        public void RequestSinglePlayerMatch(PlayerIdentity requestingPlayerIdentity, string connectionId)
        {
            Func<Task> methode = () =>
            {
                OnRequestSinglePlayerMatch?.Invoke(requestingPlayerIdentity);
                return Task.CompletedTask;
            };

            Request(requestingPlayerIdentity, methode, connectionId);
        }
        public void RequestOppoenntRoboterPlyerMatch(PlayerIdentity playerIdentity, string requestingOpponentRoboterPlayerId, string connectionId)
        {
            Func<Task> methode = () =>
            {
                OnRequestOppoenntRoboterPlyerMatch?.Invoke(requestingOpponentRoboterPlayerId);
                return Task.CompletedTask;
            };

            Request(playerIdentity, methode, connectionId);
        }
        public void AcceptOppoenntRoboterPlyerMatch(PlayerIdentity playerIdentity, string acceptingOpponentRoboterPlayerId, string connectionId)
        {
            Func<Task> methode = () =>
            {
                OnAcceptOppoenntRoboterPlyerMatch?.Invoke(acceptingOpponentRoboterPlayerId);
                return Task.CompletedTask;
            };

            Request(playerIdentity, methode, connectionId);
        }
        public void RejectOppoenntRoboterPlyerMatch(PlayerIdentity playerIdentity, string rejectingOpponentRoboterPlayerId, string connectionId)
        {
            Func<Task> methode = () =>
            {
                OnRejectOppoenntRoboterPlyerMatch?.Invoke(rejectingOpponentRoboterPlayerId);
                return Task.CompletedTask;
            };

            Request(playerIdentity, methode, connectionId);
        }
        public void ConnectToOpponentRoboterPlayer(PlayerIdentity initiatorPlayerIdentity, string hubUrl, string connectionId)
        {
            Func<Task> methode = () =>
            {
                OnConnectToOpponentRoboterPlayer?.Invoke(hubUrl, connectionId);
                return Task.CompletedTask;
            };

            Request(initiatorPlayerIdentity, methode, connectionId);
        }


        // sending
        public async Task SendUserData(string connectionId, PlayerInfoDTO userData)
        {
            await _hubConetext.Clients.Client(connectionId).SendAsync(nameof(SendUserData), userData);
        }
        public async Task SendConnectedPlayers(string connectionId, ConnectedPlayersDTO connectedPlayers)
        {
            await _hubConetext.Clients.Client(connectionId).SendAsync(nameof(SendConnectedPlayers), connectedPlayers);
        }
        public async Task SendGamePlan(string connectionId, IEnumerable<MatchDTO> gamePlan)
        {
            await _hubConetext.Clients.Client(connectionId).SendAsync(nameof(SendGamePlan), gamePlan);
        }
        public async Task SendGame(string connectionId, GameDTO game)
        {
            await _hubConetext.Clients.Client(connectionId).SendAsync(nameof(SendGame), game);
        }
        public async Task SendBestList(string connectionId, IEnumerable<GameResultDTO> bestlist)
        {
            await _hubConetext.Clients.Client(connectionId).SendAsync(nameof(SendBestList), bestlist);
        }
        public async Task SendHint(string connectionId, int column)
        {
            await _hubConetext.Clients.Client(connectionId).SendAsync(nameof(SendHint), column);
        }

        public async Task PlayerConnected(string connectionId, ConnectedPlayerDTO onlinePlayer)
        {
            await _hubConetext.Clients.Client(connectionId).SendAsync(nameof(PlayerConnected), onlinePlayer);
        }
        public async Task PlayerDisconnected(string connectionId, string playerId)
        {
            await _hubConetext.Clients.Client(connectionId).SendAsync(nameof(PlayerDisconnected), playerId);
        }
        public async Task OpponentRoboterPlayerConnected(string connectionId, ConnectedPlayerDTO opponentRoboterPlayer)
        {
            await _hubConetext.Clients.Client(connectionId).SendAsync(nameof(OpponentRoboterPlayerConnected), opponentRoboterPlayer);
        }
        public async Task OpponentRoboterPlayerDisconnected(string connectionId, string playerId)
        {
            await _hubConetext.Clients.Client(connectionId).SendAsync(nameof(OpponentRoboterPlayerDisconnected), playerId);
        }

        public async Task PlayerRequestedMatch(string connectionId, string playerId)
        {
            await _hubConetext.Clients.Client(connectionId).SendAsync(nameof(PlayerRequestedMatch), playerId);
        }
        public async Task YouRequestedMatch(string connectionId, string playerId)
        {
            await _hubConetext.Clients.Client(connectionId).SendAsync(nameof(YouRequestedMatch), playerId);
        }
        public async Task PlayerRejectedMatch(string connectionId, string playerId)
        {
            await _hubConetext.Clients.Client(connectionId).SendAsync(nameof(PlayerRejectedMatch), playerId);
        }
        public async Task YouRejectedMatch(string connectionId, string playerId)
        {
            await _hubConetext.Clients.Client(connectionId).SendAsync(nameof(YouRejectedMatch), playerId);
        }
        public async Task Matched(string connectionId, MatchDTO match)
        {
            await _hubConetext.Clients.Client(connectionId).SendAsync(nameof(Matched), match);
        }
        public async Task GameStarted(string connectionId, GameDTO connect4Game)
        {
            await _hubConetext.Clients.Client(connectionId).SendAsync(nameof(GameStarted), connect4Game);
        }
        public async Task GameEnded(string connectionId, GameResultDTO gameResult)
        {
            await _hubConetext.Clients.Client(connectionId).SendAsync(nameof(GameEnded), gameResult);
        }
        public async Task ConfirmedGameStart(string connectionId, string playerId)
        {
            await _hubConetext.Clients.Client(connectionId).SendAsync(nameof(ConfirmedGameStart), playerId);
        }
        public async Task MovePlayed(string connectionId, string playerId, FieldDTO field)
        {
            await _hubConetext.Clients.Client(connectionId).SendAsync(nameof(MovePlayed), playerId, field);
        }
        public async Task YouStoppedWatchingGame(string connectionId)
        {
            await _hubConetext.Clients.Client(connectionId).SendAsync(nameof(YouStoppedWatchingGame));
        }

        public async Task NotAbleToConnectToOpponentRoboterPlayer(string connectionId, string errorMessage)
        {
            await _hubConetext.Clients.Client(connectionId).SendAsync(nameof(NotAbleToConnectToOpponentRoboterPlayer), errorMessage);
        }

        protected override async void RequestError(string connectionId)
        {
            await _hubConetext.Clients.Client(connectionId).SendAsync(nameof(RequestError));
        }
    }
}
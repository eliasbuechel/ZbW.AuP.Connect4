using backend.Data;
using backend.services;
using backend.utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace backend.communication.signalR.frontendApi
{
    [Authorize]
    internal class FrontendHub : Hub
    {
        public FrontendHub(FrontendApi frontendApi, UserManager<PlayerIdentity> userManager)
        {
            _frontendApi = frontendApi;
            _userManager = userManager;
        }

        public void GetUserData()
        {
            _frontendApi.GetUserData(Identification, Context.ConnectionId);
        }
        public void GetConnectedPlayers()
        {
            _frontendApi.GetConnectedPlayers(Identification, Context.ConnectionId);
        }
        public void GetGamePlan()
        {
            _frontendApi.GetGamePlan(Identification, Context.ConnectionId);
        }
        public void GetGame()
        {
            _frontendApi.GetGame(Identification, Context.ConnectionId);
        }
        public void GetBestlist()
        {
            _frontendApi.GetBestlist(Identification, Context.ConnectionId);
        }
        public void GetHint()
        {
            _frontendApi.GetHint(Identification);
        }

        public void RequestMatch(string requestingPlayerId)
        {
            _frontendApi.RequestMatch(Identification, requestingPlayerId);
        }
        public void AcceptMatch(string acceptingPlayerId)
        {
            _frontendApi.AcceptMatch(Identification, acceptingPlayerId);
        }
        public void RejectMatch(string rejectingPlayerId)
        {
            _frontendApi.RejectMatch(Identification, rejectingPlayerId);
        }
        public void ConfirmGameStart()
        {
            _frontendApi.ConfirmGameStart(Identification);
        }
        public void PlayMove(int column)
        {
            _frontendApi.PlayMove(Identification, column);
        }
        public void QuitGame()
        {
            _frontendApi.QuitGame(Identification);
        }
        public void WatchGame()
        {
            _frontendApi.WatchGame(Identification);
        }
        public void StopWatchingGame()
        {
            _frontendApi.StopWatchingGame(Identification);
        }
        public void RequestSinglePlayerMatch()
        {
            _frontendApi.RequestSinglePlayerMatch(Identification);
        }
        public void RequestOppoenntRoboterPlyerMatch(string opponentRoboterPlayerId)
        {
            _frontendApi.RequestOppoenntRoboterPlyerMatch(Identification, opponentRoboterPlayerId);
        }
        public void AcceptOppoenntRoboterPlyerMatch(string opponentRoboterPlayerId)
        {
            _frontendApi.AcceptOppoenntRoboterPlyerMatch(Identification, opponentRoboterPlayerId);
        }
        public void RejectOppoenntRoboterPlyerMatch(string opponentRoboterPlayerId)
        {
            _frontendApi.RejectOppoenntRoboterPlyerMatch(Identification, opponentRoboterPlayerId);
        }
        public void ConnectToOpponentRoboterPlayer(string hubUrl)
        {
            _frontendApi.ConnectToOpponentRoboterPlayer(Identification, hubUrl);
        }
        public override Task OnConnectedAsync()
        {
            _frontendApi.Connected(Identification, Context.ConnectionId);
            return Task.CompletedTask;
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _frontendApi.Disconnected(Identification, Context.ConnectionId);
            return Task.CompletedTask;
        }

        private PlayerIdentity Identification
        {
            get
            {
                ClaimsPrincipal? claimsPrincipal = Context.User;
                Debug.Assert(claimsPrincipal != null);

                string? userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
                Debug.Assert(userId != null);

                PlayerIdentity? identity = _userManager.FindByIdAsync(userId).Result;
                Debug.Assert(identity != null);

                return identity;
            }
        }

        private readonly FrontendApi _frontendApi;
        private readonly UserManager<PlayerIdentity> _userManager;
    }
}
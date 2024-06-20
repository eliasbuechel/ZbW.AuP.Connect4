using backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Security.Claims;

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
            try
            {
                _frontendApi.GetUserData(Identification, Context.ConnectionId);
            }
            catch (InvalidIdentitficationException)
            {
                RedirectToLogin();
            }
        }
        public void GetConnectedPlayers()
        {
            try
            {
                _frontendApi.GetConnectedPlayers(Identification, Context.ConnectionId);
            }
            catch (InvalidIdentitficationException)
            {
                RedirectToLogin();
            }
        }
        public void GetGamePlan()
        {
            try
            {
                _frontendApi.GetGamePlan(Identification, Context.ConnectionId);
            }
            catch (InvalidIdentitficationException)
            {
                RedirectToLogin();
            }
        }
        public void GetGame()
        {
            try
            {
                _frontendApi.GetGame(Identification, Context.ConnectionId);
            }
            catch (InvalidIdentitficationException)
            {
                RedirectToLogin();
            }
        }
        public void GetBestlist()
        {
            try
            {
                _frontendApi.GetBestlist(Identification, Context.ConnectionId);
            }
            catch (InvalidIdentitficationException)
            {
                RedirectToLogin();
            }
        }
        public void GetHint()
        {
            try
            {
                _frontendApi.GetHint(Identification, Context.ConnectionId);
            }
            catch (InvalidIdentitficationException)
            {
                RedirectToLogin();
            }
        }

        public void RequestMatch(string requestingPlayerId)
        {
            try
            {
                _frontendApi.RequestMatch(Identification, requestingPlayerId, Context.ConnectionId);
            }
            catch (InvalidIdentitficationException)
            {
                RedirectToLogin();
            }
        }
        public void AcceptMatch(string acceptingPlayerId)
        {
            try
            {
                _frontendApi.AcceptMatch(Identification, acceptingPlayerId, Context.ConnectionId);
            }
            catch (InvalidIdentitficationException)
            {
                RedirectToLogin();
            }
        }
        public void RejectMatch(string rejectingPlayerId)
        {
            try
            {
                _frontendApi.RejectMatch(Identification, rejectingPlayerId, Context.ConnectionId);
            }
            catch (InvalidIdentitficationException)
            {
                RedirectToLogin();
            }
        }
        public void ConfirmGameStart()
        {
            try
            {
                _frontendApi.ConfirmGameStart(Identification, Context.ConnectionId);
            }
            catch (InvalidIdentitficationException)
            {
                RedirectToLogin();
            }
        }
        public void PlayMove(int column)
        {
            try
            {
                _frontendApi.PlayMove(Identification, column, Context.ConnectionId);
            }
            catch (InvalidIdentitficationException)
            {
                RedirectToLogin();
            }
        }
        public void QuitGame()
        {
            try
            {
                _frontendApi.QuitGame(Identification, Context.ConnectionId);
            }
            catch (InvalidIdentitficationException)
            {
                RedirectToLogin();
            }
        }
        public void WatchGame()
        {
            try
            {
                _frontendApi.WatchGame(Identification, Context.ConnectionId);
            }
            catch (InvalidIdentitficationException)
            {
                RedirectToLogin();
            }
        }
        public void StopWatchingGame()
        {
            try
            {
                _frontendApi.StopWatchingGame(Identification, Context.ConnectionId);
            }
            catch (InvalidIdentitficationException)
            {
                RedirectToLogin();
            }
        }
        public void RequestSinglePlayerMatch()
        {
            try
            {
                _frontendApi.RequestSinglePlayerMatch(Identification, Context.ConnectionId);
            }
            catch (InvalidIdentitficationException)
            {
                RedirectToLogin();
            }
        }
        public void RequestOppoenntRoboterPlyerMatch(string opponentRoboterPlayerId)
        {
            try
            {
                _frontendApi.RequestOppoenntRoboterPlyerMatch(Identification, opponentRoboterPlayerId, Context.ConnectionId);
            }
            catch (InvalidIdentitficationException)
            {
                RedirectToLogin();
            }
        }
        public void AcceptOppoenntRoboterPlyerMatch(string opponentRoboterPlayerId)
        {
            try
            {
                _frontendApi.AcceptOppoenntRoboterPlyerMatch(Identification, opponentRoboterPlayerId, Context.ConnectionId);
            }
            catch (InvalidIdentitficationException)
            {
                RedirectToLogin();
            }
        }
        public void RejectOppoenntRoboterPlyerMatch(string opponentRoboterPlayerId)
        {
            try
            {
                _frontendApi.RejectOppoenntRoboterPlyerMatch(Identification, opponentRoboterPlayerId, Context.ConnectionId);
            }
            catch (InvalidIdentitficationException)
            {
                RedirectToLogin();
            }
        }
        public void ConnectToOpponentRoboterPlayer(string hubUrl)
        {
            try
            {
                _frontendApi.ConnectToOpponentRoboterPlayer(Identification, hubUrl, Context.ConnectionId);
            }
            catch (InvalidIdentitficationException)
            {
                RedirectToLogin();
            }
        }
        public override Task OnConnectedAsync()
        {
            try
            {
                _frontendApi.Connected(Identification, Context.ConnectionId);
            }
            catch (InvalidIdentitficationException)
            {
                RedirectToLogin();
            }

            return Task.CompletedTask;
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                _frontendApi.Disconnected(Identification, Context.ConnectionId);
            }
            catch (InvalidIdentitficationException)
            {
                RedirectToLogin();
            }

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

                PlayerIdentity identity = _userManager.FindByIdAsync(userId).Result ?? throw new InvalidIdentitficationException();
                return identity;
            }
        }

        private void RedirectToLogin()
        {
            Clients.Caller.SendAsync("RedirectToLogin");
        }

        private readonly FrontendApi _frontendApi;
        private readonly UserManager<PlayerIdentity> _userManager;
    }

    internal class InvalidIdentitficationException : Exception { }
}
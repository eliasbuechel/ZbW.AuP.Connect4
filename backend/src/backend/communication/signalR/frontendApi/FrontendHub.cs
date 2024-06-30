using backend.data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Security.Claims;

namespace backend.communication.signalR.frontendApi
{
    [Authorize]
    internal class FrontendHub(FrontendApi frontendApi, UserManager<PlayerIdentity> userManager) : Hub
    {
        public void GetUserData()
        {
            ExecuteWithRedirectToLoginOnException(() => _frontendApi.GetUserData(Identification, Context.ConnectionId));
        }
        public void GetConnectedPlayers()
        {
            ExecuteWithRedirectToLoginOnException(() => _frontendApi.GetConnectedPlayers(Identification, Context.ConnectionId));
        }
        public void GetGamePlan()
        {
            ExecuteWithRedirectToLoginOnException(() => _frontendApi.GetGamePlan(Identification, Context.ConnectionId));
        }
        public void GetGame()
        {
            ExecuteWithRedirectToLoginOnException(() => _frontendApi.GetGame(Identification, Context.ConnectionId));
        }
        public void GetBestlist()
        {
            ExecuteWithRedirectToLoginOnException(() => _frontendApi.GetBestlist(Identification, Context.ConnectionId));
        }
        public void GetHint()
        {
            ExecuteWithRedirectToLoginOnException(() => _frontendApi.GetHint(Identification, Context.ConnectionId));
        }
        public void GetVisualisationState()
        {
            ExecuteWithRedirectToLoginOnException(() => _frontendApi.GetVisualisationState(Identification, Context.ConnectionId));
        }

        public void RequestMatch(string requestingPlayerId)
        {
            ExecuteWithRedirectToLoginOnException(() => _frontendApi.RequestMatch(Identification, requestingPlayerId, Context.ConnectionId));
        }
        public void AcceptMatch(string acceptingPlayerId)
        {
            ExecuteWithRedirectToLoginOnException(() => _frontendApi.AcceptMatch(Identification, acceptingPlayerId, Context.ConnectionId));
        }
        public void RejectMatch(string rejectingPlayerId)
        {
            ExecuteWithRedirectToLoginOnException(() => _frontendApi.RejectMatch(Identification, rejectingPlayerId, Context.ConnectionId));
        }
        public void ConfirmGameStart()
        {
            ExecuteWithRedirectToLoginOnException(() => _frontendApi.ConfirmGameStart(Identification, Context.ConnectionId));
        }
        public void PlayMove(int column)
        {
            ExecuteWithRedirectToLoginOnException(() => _frontendApi.PlayMove(Identification, column, Context.ConnectionId));
        }
        public void QuitGame()
        {
            ExecuteWithRedirectToLoginOnException(() => _frontendApi.QuitGame(Identification, Context.ConnectionId));
        }
        public void WatchGame()
        {
            ExecuteWithRedirectToLoginOnException(() => _frontendApi.WatchGame(Identification, Context.ConnectionId));
        }
        public void StopWatchingGame()
        {
            ExecuteWithRedirectToLoginOnException(() => _frontendApi.StopWatchingGame(Identification, Context.ConnectionId));
        }
        public void RequestSinglePlayerMatch()
        {
            ExecuteWithRedirectToLoginOnException(() => _frontendApi.RequestSinglePlayerMatch(Identification, Context.ConnectionId));
        }
        public void RequestOppoenntRoboterPlyerMatch(string opponentRoboterPlayerId)
        {
            ExecuteWithRedirectToLoginOnException(() => _frontendApi.RequestOppoenntRoboterPlyerMatch(Identification, opponentRoboterPlayerId, Context.ConnectionId));
        }
        public void AcceptOppoenntRoboterPlyerMatch(string opponentRoboterPlayerId)
        {
            ExecuteWithRedirectToLoginOnException(() => _frontendApi.AcceptOppoenntRoboterPlyerMatch(Identification, opponentRoboterPlayerId, Context.ConnectionId));
        }
        public void RejectOppoenntRoboterPlyerMatch(string opponentRoboterPlayerId)
        {
            ExecuteWithRedirectToLoginOnException(() => _frontendApi.RejectOppoenntRoboterPlyerMatch(Identification, opponentRoboterPlayerId, Context.ConnectionId));
        }
        public void ConnectToOpponentRoboterPlayer(string hubUrl)
        {
            ExecuteWithRedirectToLoginOnException(() => _frontendApi.ConnectToOpponentRoboterPlayer(Identification, hubUrl, Context.ConnectionId));
        }
        public void VisualizeOnRoboterChanged(bool isVisualizingOnRoboter)
        {
            ExecuteWithRedirectToLoginOnException(() => _frontendApi.VisualizeOnRoboterChanged(Identification, Context.ConnectionId, isVisualizingOnRoboter));
        }

        public override Task OnConnectedAsync()
        {
            ExecuteWithRedirectToLoginOnException(() => _frontendApi.Connected(Identification, Context.ConnectionId));
            return Task.CompletedTask;
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            ExecuteWithRedirectToLoginOnException(() => _frontendApi.Disconnected(Identification, Context.ConnectionId));
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

        private void ExecuteWithRedirectToLoginOnException(Action action)
        {
            try
            {
                action();
            }
            catch (InvalidIdentitficationException)
            {
                RedirectToLogin();
            }
        }
        private void RedirectToLogin()
        {
            Clients.Caller.SendAsync("RedirectToLogin");
        }

        private readonly FrontendApi _frontendApi = frontendApi;
        private readonly UserManager<PlayerIdentity> _userManager = userManager;
    }
}
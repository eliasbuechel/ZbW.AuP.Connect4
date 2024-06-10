using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace backend.communication.signalR.opponentRoboterApi
{
    internal class OpponentRoboterHub : Hub
    { 
        public OpponentRoboterHub(OpponentRoboterHubApi opponentRoboterHubApi)
        {
            _opponentRoboterHubApi = opponentRoboterHubApi;
        }

        public void RequestMatch()
        {
            _opponentRoboterHubApi.RequestMatch(Context.ConnectionId);
        }
        public void AcceptMatch()
        {
            _opponentRoboterHubApi.AcceptMatch(Context.ConnectionId);
        }
        public void RejectMatch()
        {
            _opponentRoboterHubApi.RejectMatch(Context.ConnectionId);
        }
        public void ConfirmGameStart()
        {
            _opponentRoboterHubApi.ConfirmGameStart(Context.ConnectionId);
        }
        public void PlayMove(int column)
        {
            _opponentRoboterHubApi.PlayMove(Context.ConnectionId, column);
        }
        public void QuitGame()
        {
            _opponentRoboterHubApi.QuitGame(Context.ConnectionId);
        }

        public override Task OnConnectedAsync()
        {
            _opponentRoboterHubApi.Connected(CallerUrl, Context.ConnectionId);
            return Task.CompletedTask;
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _opponentRoboterHubApi.Disconnected(CallerUrl, Context.ConnectionId);
            return Task.CompletedTask;
        }

        private string CallerUrl
        {
            get
            {
                var callerUrl = Context.GetHttpContext()?.Request.GetDisplayUrl();

                if (callerUrl == null)
                    callerUrl = "Opponent URL not found!";

                return callerUrl;
            }
        }

        private readonly OpponentRoboterHubApi _opponentRoboterHubApi;
    }
}
using backend.Infrastructure;
using backend.utilities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

namespace backend.communication.signalR.opponentRoboterApi
{
    internal class OpponentRoboterHubApi(
        IHubContext<OpponentRoboterHub> opponentRoboterHubContext,
        RequestHandlerManager<string> requestHandlerManager
            ) : OpponentRoboterApi(requestHandlerManager)
    {

        // sending
        public async void Send_RequestMatch(string connectionId)
        {
            LogSend(nameof(RequestMatch));
            await _opponentRoboterHubConetxt.Clients.Client(connectionId).SendAsync(nameof(RequestMatch));
        }

        public async void Send_AcceptMatch(string connectionId)
        {
            LogSend(nameof(AcceptMatch));
            await _opponentRoboterHubConetxt.Clients.Client(connectionId).SendAsync(nameof(AcceptMatch));
        }
        public async void Send_RejectMatch(string connectionId)
        {
            LogSend(nameof(RejectMatch));
            await _opponentRoboterHubConetxt.Clients.Client(connectionId).SendAsync(nameof(RejectMatch));
        }
        public async void Send_ConfirmGameStart(string connectionId)
        {
            LogSend(nameof(ConfirmGameStart));
            await _opponentRoboterHubConetxt.Clients.Client(connectionId).SendAsync(nameof(ConfirmGameStart));
        }
        public async void Send_PlayMove(string connectionId, int column)
        {
            LogSend(nameof(PlayMove), column.ToString());
            await _opponentRoboterHubConetxt.Clients.Client(connectionId).SendAsync(nameof(PlayMove), column);
        }
        public async void Send_QuitGame(string connectionId)
        {
            LogSend(nameof(QuitGame));
            await _opponentRoboterHubConetxt.Clients.Client(connectionId).SendAsync(nameof(QuitGame));
        }

        protected override LogContext LogContext => LogContext.OPPONENT_ROBOTER_CLIENT_API;

        private readonly IHubContext<OpponentRoboterHub> _opponentRoboterHubConetxt = opponentRoboterHubContext;
    }
}
using backend.infrastructure;
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
        public async Task Send_RequestMatch(string connectionId)
        {
            LogSend(nameof(RequestMatch));
            await _opponentRoboterHubConetxt.Clients.Client(connectionId).SendAsync(nameof(RequestMatch));
        }
        public async Task Send_AcceptMatch(string connectionId)
        {
            LogSend(nameof(AcceptMatch));
            await _opponentRoboterHubConetxt.Clients.Client(connectionId).SendAsync(nameof(AcceptMatch));
        }
        public async Task Send_RejectMatch(string connectionId)
        {
            LogSend(nameof(RejectMatch));
            await _opponentRoboterHubConetxt.Clients.Client(connectionId).SendAsync(nameof(RejectMatch));
        }
        public async Task Send_ConfirmGameStart(string connectionId)
        {
            LogSend(nameof(ConfirmGameStart));
            await _opponentRoboterHubConetxt.Clients.Client(connectionId).SendAsync(nameof(ConfirmGameStart));
        }
        public async Task Send_PlayMove(string connectionId, int column)
        {
            LogSend(nameof(PlayMove), column.ToString());
            await _opponentRoboterHubConetxt.Clients.Client(connectionId).SendAsync(nameof(PlayMove), column);
        }
        public async Task Send_QuitGame(string connectionId)
        {
            LogSend(nameof(QuitGame));
            await _opponentRoboterHubConetxt.Clients.Client(connectionId).SendAsync(nameof(QuitGame));
        }

        protected override LogContext LogContext => LogContext.OPPONENT_ROBOTER_HUB_API;

        private readonly IHubContext<OpponentRoboterHub> _opponentRoboterHubConetxt = opponentRoboterHubContext;
    }
}
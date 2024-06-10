using backend.utilities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

namespace backend.communication.signalR.opponentRoboterApi
{
    internal class OpponentRoboterHubApi : OpponentRoboterApi
    {
        public OpponentRoboterHubApi(
            IHubContext<OpponentRoboterHub> opponentRoboterHubContext,
            RequestHandlerManager<string> requestHandlerManager
            ) : base(requestHandlerManager)
        {
            _opponentRoboterHubConetxt = opponentRoboterHubContext;
        }

        // sending
        public async void Send_RequestMatch(string connectionId)
        {
            await _opponentRoboterHubConetxt.Clients.Client(connectionId).SendAsync(nameof(RequestMatch));
        }
        public async void Send_AcceptMatch(string connectionId)
        {
            await _opponentRoboterHubConetxt.Clients.Client(connectionId).SendAsync(nameof(AcceptMatch));
        }
        public async void Send_RejectMatch(string connectionId)
        {
            await _opponentRoboterHubConetxt.Clients.Client(connectionId).SendAsync(nameof(RejectMatch));
        }
        public async void Send_ConfirmGameStart(string connectionId)
        {
            await _opponentRoboterHubConetxt.Clients.Client(connectionId).SendAsync(nameof(ConfirmGameStart));
        }
        public async void Send_PlayMove(string connectionId, int column)
        {
            await _opponentRoboterHubConetxt.Clients.Client(connectionId).SendAsync(nameof(PlayMove), column);
        }
        public async void Send_QuitGame(string connectionId)
        {
            await _opponentRoboterHubConetxt.Clients.Client(connectionId).SendAsync(nameof(QuitGame));
        }

        private readonly IHubContext<OpponentRoboterHub> _opponentRoboterHubConetxt;
    }
}
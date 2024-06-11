using backend.utilities;
using Microsoft.AspNetCore.SignalR;

namespace backend.communication.signalR
{
    internal delegate void Connected<TIdentification>(TIdentification identification, string connectionId);
    internal delegate void Disconnected<TIdentification>(TIdentification identification, string connectionId);

    internal class SignalRApi<THub, TIdentification> where THub : Hub where TIdentification : class
    {
        public SignalRApi(IHubContext<THub> hubClient, RequestHandlerManager<TIdentification> requestHandlerManager)
        {
            _hubConetext = hubClient;
            _requestHandlerManager = requestHandlerManager;
        }

        public event Connected<TIdentification>? OnConnected;
        public event Disconnected<TIdentification>? OnDisconnected;

        public void Connected(TIdentification identification, string connectionId)
        {
            _requestHandlerManager.GetOrCreateHandler(identification).Enqueue(() =>
            {
                OnConnected?.Invoke(identification, connectionId);
                return Task.CompletedTask;
            });
        }
        public void Disconnected(TIdentification identification, string connectionId)
        {
            _requestHandlerManager.GetOrCreateHandler(identification).Enqueue(() =>
            {
                OnDisconnected?.Invoke(identification, connectionId);
                return Task.CompletedTask;
            });
        }

        protected readonly IHubContext<THub> _hubConetext;
        protected readonly RequestHandlerManager<TIdentification> _requestHandlerManager;
    }
}
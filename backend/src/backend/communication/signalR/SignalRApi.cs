using backend.utilities;
using Microsoft.AspNetCore.SignalR;

namespace backend.communication.signalR
{
    internal delegate void Connected<in TIdentification>(TIdentification identification, string connectionId);
    internal delegate void Disconnected<in TIdentification>(TIdentification identification, string connectionId);

    internal class SignalRApi<THub, TIdentification>(IHubContext<THub> hubClient, RequestHandlerManager<TIdentification> requestHandlerManager) where THub : Hub where TIdentification : class
    {
        public event Connected<TIdentification>? OnConnected;
        public event Disconnected<TIdentification>? OnDisconnected;

        public void Connected(TIdentification identification, string connectionId)
        {
            Task methode()
            {
                OnConnected?.Invoke(identification, connectionId);
                return Task.CompletedTask;
            }

            Request(identification, methode, connectionId);
        }
        public void Disconnected(TIdentification identification, string connectionId)
        {
            Task methode()
            {
                OnDisconnected?.Invoke(identification, connectionId);
                return Task.CompletedTask;
            }

            Request(identification, methode, connectionId);
        }

        protected void Request(TIdentification identification, Func<Task> methode, string connectionId)
        {
            RequestHandler requestHandler = _requestHandlerManager.GetOrCreateHandler(identification);
            requestHandler.OnRequestError += RequestError;
            requestHandler.Enqueue(methode, connectionId);
        }
        protected virtual void RequestError(string connectionId)
        { }

        protected readonly IHubContext<THub> _hubConetext = hubClient;
        private readonly RequestHandlerManager<TIdentification> _requestHandlerManager = requestHandlerManager;
    }
}
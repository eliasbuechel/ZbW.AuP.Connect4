using backend.Infrastructure;
using backend.utilities;

namespace backend.communication.signalR.opponentRoboterApi
{
    internal delegate void Connected(string callerUrl, string connectionId);
    internal delegate void Disconnected(string callerUrl, string connectionId);
    internal delegate void RequestMatch(string connectionId);
    internal delegate void AcceptMatch(string connectionId);
    internal delegate void RejectMatch(string connectionId);
    internal delegate void ConfirmGameStart(string connectionId);
    internal delegate void PlayMove(string connectionId, int column);
    internal delegate void QuitGame(string connectionId);

    internal abstract class OpponentRoboterApi(RequestHandlerManager<string> requestHandlerManager)
    {
        public event Connected? OnConnected;
        public event Disconnected? OnDisconnected;
        public event RequestMatch? OnRequestMatch;
        public event AcceptMatch? OnAcceptMatch;
        public event RejectMatch? OnRejectMatch;
        public event ConfirmGameStart? OnConfirmGameStart;
        public event PlayMove? OnPlayMove;
        public event QuitGame? OnQuitGame;

        // receving
        public void Connected(string callerUrl, string connectionId)
        {
            LogRecive(nameof(Connected));
            _requestHandlerManager.GetOrCreateHandler(connectionId).Enqueue(() =>
            {
                OnConnected?.Invoke(callerUrl, connectionId);
                return Task.CompletedTask;
            }, connectionId);
        }
        public void Disconnected(string callerUrl, string connectionId)
        {
            LogRecive(nameof(Disconnected));
            _requestHandlerManager.GetOrCreateHandler(connectionId).Enqueue(() =>
            {
                OnDisconnected?.Invoke(callerUrl, connectionId);
                return Task.CompletedTask;
            }, connectionId);
        }
        public void RequestMatch(string connectionId)
        {
            LogRecive(nameof(RequestMatch));
            _requestHandlerManager.GetOrCreateHandler(connectionId).Enqueue(() =>
            {
                OnRequestMatch?.Invoke(connectionId);
                return Task.CompletedTask;
            }, connectionId);
        }
        public void AcceptMatch(string connectionId)
        {
            LogRecive(nameof(AcceptMatch));
            _requestHandlerManager.GetOrCreateHandler(connectionId).Enqueue(() =>
            {
                OnAcceptMatch?.Invoke(connectionId);
                return Task.CompletedTask;
            }, connectionId);
        }
        public void RejectMatch(string connectionId)
        {
            LogRecive(nameof(RejectMatch));
            _requestHandlerManager.GetOrCreateHandler(connectionId).Enqueue(() =>
            {
                OnRejectMatch?.Invoke(connectionId);
                return Task.CompletedTask;
            }, connectionId);
        }
        public void ConfirmGameStart(string connectionId)
        {
            LogRecive(nameof(ConfirmGameStart));
            _requestHandlerManager.GetOrCreateHandler(connectionId).Enqueue(() =>
            {
                OnConfirmGameStart?.Invoke(connectionId);
                return Task.CompletedTask;
            }, connectionId);
        }
        public void PlayMove(string connectionId, int column)
        {
            LogRecive(nameof(PlayMove), column.ToString());
            _requestHandlerManager.GetOrCreateHandler(connectionId).Enqueue(() =>
            {
                OnPlayMove?.Invoke(connectionId, column);
                return Task.CompletedTask;
            }, connectionId);
        }
        public void QuitGame(string connectionId)
        {
            LogRecive(nameof(QuitGame));
            _requestHandlerManager.GetOrCreateHandler(connectionId).Enqueue(() =>
            {
                OnQuitGame?.Invoke(connectionId);
                return Task.CompletedTask;
            }, connectionId);
        }

        private void Log(string message)
        {
            Logger.Log(LogLevel.Debug, LogContext, message);
        }
        protected void LogSend(string methodeName, string? data = null)
        {
            if (data == null)
                Log($"Send on '{methodeName}'.");
            else
                Log($"Send on '{methodeName}':[{data}]");
        }
        protected void LogRecive(string methodeName, string? data = null)
        {
            if (data == null)
                Log($"Recive on '{methodeName}'.");
            else
                Log($"Recive on '{methodeName}':[{data}]");
        }

        protected abstract LogContext LogContext {get;}

        private readonly RequestHandlerManager<string> _requestHandlerManager = requestHandlerManager;
    }
}
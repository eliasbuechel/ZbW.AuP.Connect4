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

    internal abstract class OpponentRoboterApi
    {
        public event Connected? OnConnected;
        public event Disconnected? OnDisconnected;
        public event RequestMatch? OnRequestMatch;
        public event AcceptMatch? OnAcceptMatch;
        public event RejectMatch? OnRejectMatch;
        public event ConfirmGameStart? OnConfirmGameStart;
        public event PlayMove? OnPlayMove;
        public event QuitGame? OnQuitGame;

        public OpponentRoboterApi(RequestHandlerManager<string> requestHandlerManager)
        {
            _requestHandlerManager = requestHandlerManager;
        }

        // receving
        public void Connected(string callerUrl, string connectionId)
        {
            _requestHandlerManager.GetOrCreateHandler(connectionId).Enqueue(() =>
            {
                OnConnected?.Invoke(callerUrl, connectionId);
                return Task.CompletedTask;
            }, connectionId);
        }
        public void Disconnected(string callerUrl, string connectionId)
        {
            _requestHandlerManager.GetOrCreateHandler(connectionId).Enqueue(() =>
            {
                OnDisconnected?.Invoke(callerUrl, connectionId);
                return Task.CompletedTask;
            }, connectionId);
        }
        public void RequestMatch(string connectionId)
        {
            _requestHandlerManager.GetOrCreateHandler(connectionId).Enqueue(() =>
            {
                OnRequestMatch?.Invoke(connectionId);
                return Task.CompletedTask;
            }, connectionId);
        }
        public void AcceptMatch(string connectionId)
        {
            _requestHandlerManager.GetOrCreateHandler(connectionId).Enqueue(() =>
            {
                OnAcceptMatch?.Invoke(connectionId);
                return Task.CompletedTask;
            }, connectionId);
        }
        public void RejectMatch(string connectionId)
        {
            _requestHandlerManager.GetOrCreateHandler(connectionId).Enqueue(() =>
            {
                OnRejectMatch?.Invoke(connectionId);
                return Task.CompletedTask;
            }, connectionId);
        }
        public void ConfirmGameStart(string connectionId)
        {
            _requestHandlerManager.GetOrCreateHandler(connectionId).Enqueue(() =>
            {
                OnConfirmGameStart?.Invoke(connectionId);
                return Task.CompletedTask;
            }, connectionId);
        }
        public void PlayMove(string connectionId, int column)
        {
            _requestHandlerManager.GetOrCreateHandler(connectionId).Enqueue(() =>
            {
                OnPlayMove?.Invoke(connectionId, column);
                return Task.CompletedTask;
            }, connectionId);
        }
        public void QuitGame(string connectionId)
        {
            _requestHandlerManager.GetOrCreateHandler(connectionId).Enqueue(() =>
            {
                OnQuitGame?.Invoke(connectionId);
                return Task.CompletedTask;
            }, connectionId);
        }

        private readonly RequestHandlerManager<string> _requestHandlerManager;
    }
}
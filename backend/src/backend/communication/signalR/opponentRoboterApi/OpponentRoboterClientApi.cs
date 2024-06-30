using backend.infrastructure;
using backend.utilities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace backend.communication.signalR.opponentRoboterApi
{
    internal class OpponentRoboterClientApi : OpponentRoboterApi
    {
        public OpponentRoboterClientApi(RequestHandlerManager<string> requestHandlerManager, string hubUrl) : base(requestHandlerManager)
        {
            _hubUrl = hubUrl;

            _connection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .Build();

            _connection.Closed += OnConnectionClosed;

            _requestMatchHandler = _connection.On(nameof(RequestMatch), RequestMatch);
            _acceptMatchHandler = _connection.On(nameof(AcceptMatch), AcceptMatch);
            _rejectMatchHandler = _connection.On(nameof(RejectMatch), RejectMatch);
            _confirmGameStartHandler = _connection.On(nameof(ConfirmGameStart), ConfirmGameStart);
            _playMoveHandler = _connection.On<int>(nameof(PlayMove), PlayMove);
            _quitGameHandler = _connection.On(nameof(QuitGame), QuitGame);

            Connect().Wait();
        }

        // reciving
        private void RequestMatch()
        {
            RequestMatch(_connectionId);
        }
        private void AcceptMatch()
        {
            AcceptMatch(_connectionId);
        }
        private void RejectMatch()
        {
            RejectMatch(_connectionId);
        }
        private void ConfirmGameStart()
        {
            ConfirmGameStart(_connectionId);
        }
        private void PlayMove(int column)
        {
            PlayMove(_connectionId, column);
        }
        private void QuitGame()
        {
            QuitGame(_connectionId);
        }

        // sending
        public async Task Send_RequestMatch()
        {
            LogSend(nameof(RequestMatch));
            await _connection.SendAsync(nameof(RequestMatch));
        }
        public async Task Send_AcceptMatch()
        {
            LogSend(nameof(AcceptMatch));
            await _connection.SendAsync(nameof(AcceptMatch));
        }
        public async Task Send_RejectMatch()
        {
            LogSend(nameof(RejectMatch));
            await _connection.SendAsync(nameof(RejectMatch));
        }
        public async Task Send_ConfirmGameStart()
        {
            LogSend(nameof(ConfirmGameStart));
            await _connection.SendAsync(nameof(ConfirmGameStart));
        }
        public async Task Send_PlayMove(int column)
        {
            LogSend(nameof(PlayMove), column.ToString());
            await _connection.SendAsync(nameof(PlayMove), column);
        }
        public async Task Send_QuitGame()
        {
            LogSend(nameof(QuitGame));
            await _connection.SendAsync(nameof(QuitGame));
        }

        private async Task Connect()
        {
            try
            {
                await _connection.StartAsync();

                string? connectionId = _connection.ConnectionId;
                if (connectionId == null)
                {
                    Debug.Assert(false);
                    connectionId = "";
                }
                _connectionId = connectionId;

                Connected(_hubUrl, _connectionId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Not able to connect to the signalRHub: {_hubUrl} Inner error: {ex.Message}");
                Debug.Assert(false);
                Dispose();
            }

        }
        private async Task Disconnect()
        {
            await _connection.StopAsync();
            Disconnected(_hubUrl, _connectionId);
        }
        private Task OnConnectionClosed(Exception? exception)
        {
            Disconnected(_hubUrl, _connectionId);
            return Task.CompletedTask;
        }

        protected override async void OnDispose()
        {
            await Disconnect();
            _connection.Closed -= OnConnectionClosed;

            _requestMatchHandler.Dispose();
            _acceptMatchHandler.Dispose();
            _rejectMatchHandler.Dispose();
            _confirmGameStartHandler.Dispose();
            _playMoveHandler.Dispose();
            _quitGameHandler.Dispose();

            await _connection.DisposeAsync();
        }

        protected override LogContext LogContext => LogContext.OPPONENT_ROBOTER_CLIENT_API;

        private readonly string _hubUrl;
        private readonly HubConnection _connection;
        private string _connectionId = "";
        private readonly IDisposable _requestMatchHandler;
        private readonly IDisposable _acceptMatchHandler;
        private readonly IDisposable _rejectMatchHandler;
        private readonly IDisposable _confirmGameStartHandler;
        private readonly IDisposable _playMoveHandler;
        private readonly IDisposable _quitGameHandler;

    }
}
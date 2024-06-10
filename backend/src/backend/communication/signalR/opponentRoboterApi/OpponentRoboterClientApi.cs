using backend.utilities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace backend.communication.signalR.opponentRoboterApi
{
    internal class OpponentRoboterClientApi : OpponentRoboterApi, IDisposable
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
            RequestMatch(_hubUrl);
        }
        private void AcceptMatch()
        {
            AcceptMatch(_hubUrl);
        }
        private void RejectMatch()
        {
            RejectMatch(_hubUrl);
        }
        private void ConfirmGameStart()
        {
            ConfirmGameStart(_hubUrl);
        }
        private void PlayMove(int column)
        {
            PlayMove(_hubUrl, column);
        }
        private void QuitGame()
        {
            QuitGame(_hubUrl);
        }

        // sending
        public async void Send_RequestMatch()
        {
            await _connection.SendAsync(nameof(RequestMatch));
        }
        public async void Send_AcceptMatch()
        {
            await _connection.SendAsync(nameof(AcceptMatch));
        }
        public async void Send_RejectMatch()
        {
            await _connection.SendAsync(nameof(RejectMatch));
        }
        public async void Send_ConfirmGameStart()
        {
            await _connection.SendAsync(nameof(ConfirmGameStart));
        }
        public async void Send_PlayMove(int column)
        {
            await _connection.SendAsync(nameof(PlayMove));
        }
        public async void Send_QuitGame()
        {
            await _connection.SendAsync(nameof(QuitGame));
        }

        private async Task Connect()
        {
            try
            {
                await _connection.StartAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Not able to connect to the signalRHub: {_hubUrl} Inner error: {ex.Message}");
                Debug.Assert(false);
                Dispose();
            }

        }
        private async void Disconnect()
        {
            await _connection.StopAsync();
        }
        private Task OnConnectionClosed(Exception? exception)
        {
            Disconnected(_hubUrl, _hubUrl);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                Debug.Assert(false);
                return;
            }

            _disposed = true;

            Disconnect();
            _connection.Closed -= OnConnectionClosed;

            _requestMatchHandler.Dispose();
            _acceptMatchHandler.Dispose();
            _rejectMatchHandler.Dispose();
            _confirmGameStartHandler.Dispose();
            _playMoveHandler.Dispose();
            _quitGameHandler.Dispose();

            _connection.DisposeAsync();
        }

        private bool _disposed;
        private readonly string _hubUrl;
        private readonly HubConnection _connection;
        private readonly IDisposable _requestMatchHandler;
        private readonly IDisposable _acceptMatchHandler;
        private readonly IDisposable _rejectMatchHandler;
        private readonly IDisposable _confirmGameStartHandler;
        private readonly IDisposable _playMoveHandler;
        private readonly IDisposable _quitGameHandler;
    }
}
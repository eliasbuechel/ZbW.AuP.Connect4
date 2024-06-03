using backend.game;
using backend.services;
using Microsoft.AspNetCore.SignalR.Client;

namespace backend.communication.signalR
{
    internal class OpponentRoboterPlayerHubClient : Player
    {
        public OpponentRoboterPlayerHubClient(
            string hubUrl,
            AlgorythmPlayerManager algorythmPlayerManager,
            Func<IPlayer, AlgorythmPlayer> createAlgorythmPlayer,
            GameManager gameManager
            ) : base(Guid.NewGuid().ToString(), hubUrl, gameManager)
        {
            _hubUrl = hubUrl;
            _algorythmPlayerManager = algorythmPlayerManager;
            _createAlgorythmPlayer = createAlgorythmPlayer;

            _connection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .Build();

            _connection.Closed += Closed;

            _connection.On(nameof(RequestMatch), ReqeustMatchHandler);
            _connection.On(nameof(AcceptMatch), AcceptMatchHandler);
            _connection.On(nameof(RejectedMatch), RejectMatchHandler);
            _connection.On(nameof(ConfirmGameStart), ConfirmGameStartHandler);
            _connection.On<int>(nameof(PlayMove), PlayMoveHandler);
            _connection.On(nameof(QuitGame), QuitGameHandler);

            StartAsync().Wait();
        }

        // reciving
        private async void ReqeustMatchHandler()
        {
            AlgorythmPlayer algorythmPlayer = _algorythmPlayerManager.ConnectPlayer(this, _createAlgorythmPlayer);
            await RequestMatch(algorythmPlayer);
        }
        private async void AcceptMatchHandler()
        {
            AlgorythmPlayer algorythmPlayer = _algorythmPlayerManager.GetConnectedPlayerByIdentification(this);
            await AcceptMatch(algorythmPlayer);
        }
        private async void RejectMatchHandler()
        {
            AlgorythmPlayer algorythmPlayer = _algorythmPlayerManager.GetConnectedPlayerByIdentification(this);
            await RejectMatch(algorythmPlayer);
        }
        private async void ConfirmGameStartHandler()
        {
            await ConfirmGameStart();
        }
        private async void PlayMoveHandler(int column)
        {
            await PlayMove(column);
        }
        private async void QuitGameHandler()
        {
            await QuitGame();
        }

        // sending
        protected override async Task PlayerRequestedMatch(string connection, string playerId)
        {
            await _connection.SendAsync(nameof(RequestMatch));
        }

        public string HubUrl => _hubUrl;

        private Task Closed(Exception? exception)
        {
            Console.WriteLine("SignalR Client disconnected.");
            return Task.CompletedTask;
        }

        public async Task StartAsync()
        {
            Console.WriteLine("SignalR Client connecting...");
            await _connection.StartAsync();
        }
        public async Task StopAsync()
        {
            Console.WriteLine("SignalR Client disconnecting...");
            await _connection.StopAsync();
        }



        private readonly HubConnection _connection;
        private readonly string _hubUrl;
        private readonly AlgorythmPlayerManager _algorythmPlayerManager;
        private readonly Func<IPlayer, AlgorythmPlayer> _createAlgorythmPlayer;
    }
}

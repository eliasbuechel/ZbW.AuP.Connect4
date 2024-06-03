using backend.communication.DOTs;
using backend.game;
using backend.game.entities;
using backend.services;
using Microsoft.AspNetCore.SignalR;
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

        public event Action<OpponentRoboterPlayerHubClient>? OnDisconnected;
        public string HubUrl => _hubUrl;

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
        public override async void RequestedMatch(IPlayer player)
        {
            await _connection.SendAsync(nameof(RequestMatch));
        }
        public override async void Matched(Match match)
        {
            await _connection.SendAsync(nameof(AcceptMatch));
        }
        public override async void RejectedMatch(IPlayer player)
        {
            await _connection.SendAsync(nameof(RejectMatch));
        }
        public override async void ConfirmedGameStart(IPlayer player)
        {
            await _connection.SendAsync(nameof(ConfirmGameStart));
        }
        public override async void MovePlayed(IPlayer player, Field field)
        {
            await _connection.SendAsync(nameof(PlayMove), field.Column);
        }
        public override async void GameEnded(GameResult gameResult)
        {
            await _connection.SendAsync(nameof(QuitGame));
        }

        private Task Closed(Exception? exception)
        {
            OnDisconnected?.Invoke(this);
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

using backend.game;
using backend.services;
using Microsoft.AspNetCore.SignalR.Client;

namespace backend.communication.signalR
{
    internal class OpponentRoboterPlayerHubClient : Player
    {
        public OpponentRoboterPlayerHubClient(string hubUrl, GameManager gameManager) : base(Guid.NewGuid().ToString(), hubUrl, gameManager)
        {
            _hubUrl = hubUrl;

            _connection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .Build();

            _connection.Closed += Closed;
            //_connection.On<string>(nameof(PlayerRequestedMatch), PlayerRequestedMatchHandler);



            //_connection.On(nameof(RequestMatch), RequestMatch);
            //_connection.On(nameof(AcceptMatchAsync), AcceptMatchAsync);
            //_connection.On(nameof(RejectedMatch), RejectedMatch);
            //_connection.On(nameof(GetGamePlan), GetGamePlan);
            //_connection.On(nameof(GetGame), GetGame);
            //_connection.On(nameof(GetUserData), GetUserData);
            //_connection.On(nameof(GetOnlinePlayers), GetOnlinePlayers);
            //_connection.On(nameof(GetCurrentGame), GetCurrentGame);
            //_connection.On(nameof(ConfirmGameStart), ConfirmGameStart);
            //_connection.On(nameof(PlayMove), PlayMove);
            //_connection.On(nameof(QuitGame), QuitGame);

            StartAsync().Wait();
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
            //ConnectAsync(_connection.ConnectionId);
        }
        public async Task StopAsync()
        {
            Console.WriteLine("SignalR Client disconnecting...");
            await _connection.StopAsync();
        }

        //private async void PlayerRequestedMatchHandler(string playerId)
        //{
        //    await _opponentAlgorythmPlayer.RequestMatch(this);
        //}

        //protected override Task PlayerConnected(string connection, OnlinePlayerDTO onlinePlayer)
        //{
        //    return Task.CompletedTask;
        //}
        //protected override Task PlayerDisconnected(string connection, string playerId)
        //{
        //    return Task.CompletedTask;
        //}
        //protected override Task PlayerRequestedMatch(string connection, string playerId)
        //{
        //    return Task.CompletedTask;
        //}
        //protected override Task PlayerRejectedMatch(string connection, string playerId)
        //{
        //    return Task.CompletedTask;
        //}
        //protected override Task Matched(string connection, MatchDTO match)
        //{
        //    return Task.CompletedTask;
        //}
        //protected override Task MatchingEnded(string connection, string matchId)
        //{
        //    return Task.CompletedTask;
        //}
        //protected override Task GameStarted(string connection, Connect4GameDTO connect4Game)
        //{
        //    return Task.CompletedTask;
        //}
        //protected override Task GameEnded(string connection, GameResultDTO gameResult)
        //{
        //    return Task.CompletedTask;
        //}
        //protected override Task MovePlayed(string connection, string playerId, FieldDTO field)
        //{
        //    return Task.CompletedTask;
        //}
        //protected override Task SendUserData(string connection, PlayerInfoDTO userData)
        //{
        //    return Task.CompletedTask;
        //}
        //protected override Task SendOnlinePlayers(string connection, IEnumerable<OnlinePlayerDTO> onlinePlayers)
        //{
        //    return Task.CompletedTask;
        //}
        //protected override Task SendGamePlan(string connection, IEnumerable<MatchDTO> gamePlan)
        //{
        //    return Task.CompletedTask;
        //}
        //protected override Task SendGame(string connection, Connect4GameDTO game)
        //{
        //    return Task.CompletedTask;
        //}
        //protected override Task SendBestList(string connection, IEnumerable<GameResult> bestlist)
        //{
        //    return Task.CompletedTask;
        //}
        //protected override Task YouRequestedMatch(string connection, string playerId)
        //{
        //    return Task.CompletedTask;
        //}
        //protected override Task YouRejectedMatch(string connection, string playerId)
        //{
        //    return Task.CompletedTask;
        //}
        //protected override Task OpponentConfirmedGameStart(string connection)
        //{
        //    return Task.CompletedTask;
        //}
        //protected override Task GameStartConfirmed(string connection)
        //{
        //    return Task.CompletedTask;
        //}
        //protected override Task YouConfirmedGameStart(string connection)
        //{
        //    return Task.CompletedTask;
        //}
        //protected override Task SendHint(string connection, int hint)
        //{
        //    return Task.CompletedTask;
        //}

        private readonly HubConnection _connection;
        private readonly string _hubUrl;
    }
}

using backend.communication.DOTs;
using backend.Data;
using backend.game;
using backend.game.entities;
using backend.services;
using Microsoft.AspNetCore.SignalR;
using MySqlX.XDevAPI;

namespace backend.communication.signalR
{
    internal class ToPlayerHub<THub> : Player where THub : Hub
    {
        public ToPlayerHub(string playerId, string username, GameManager gameManager, IHubContext<THub> hubContext) : base(playerId, username, gameManager)
        {
            _hubContext = hubContext;
        }

        protected override async Task PlayerConnected(string connection, OnlinePlayerDTO onlinePlayer)
        {
            await _hubContext.Clients.Client(connection).SendAsync(nameof(PlayerConnected), onlinePlayer);
        }
        protected override async Task PlayerDisconnected(string connection, string playerId)
        {
            await _hubContext.Clients.Client(connection).SendAsync(nameof(PlayerDisconnected), playerId);
        }
        protected override async Task PlayerRequestedMatch(string connection, string playerId)
        {
            await _hubContext.Clients.Client(connection).SendAsync(nameof(PlayerRequestedMatch), playerId);
        }
        protected override async Task PlayerRejectedMatch(string connection, string playerId)
        {
            await _hubContext.Clients.Client(connection).SendAsync(nameof(PlayerRejectedMatch), playerId);
        }
        protected override async Task Matched(string connection, MatchDTO match)
        {
            await _hubContext.Clients.Client(connection).SendAsync(nameof(Matched), match);
        }
        protected override async Task MatchingEnded(string connection, string matchId)
        {
            await _hubContext.Clients.Client(connection).SendAsync(nameof(MatchingEnded), matchId);
        }
        protected override async Task MovePlayed(string connection, string playerId, FieldDTO field)
        {
            await _hubContext.Clients.Client(connection).SendAsync(nameof(MovePlayed), playerId, field);
        }
        protected override async Task GameStarted(string connection, Connect4GameDTO connect4Game)
        {
            await _hubContext.Clients.Client(connection).SendAsync(nameof(GameStarted), connect4Game);
        }
        protected override async Task GameEnded(string connection, GameResultDTO gameResult)
        {
            await _hubContext.Clients.Client(connection).SendAsync(nameof(GameEnded), gameResult);
        }
        protected override async Task SendUserData(string connection, PlayerInfoDTO userData)
        {
            await _hubContext.Clients.Client(connection).SendAsync(nameof(SendUserData), userData);
        }
        protected override async Task SendOnlinePlayers(string connection, IEnumerable<OnlinePlayerDTO> onlinePlayers)
        {
            await _hubContext.Clients.Client(connection).SendAsync(nameof(SendOnlinePlayers), onlinePlayers);
        }
        protected override async Task SendGamePlan(string connection, IEnumerable<MatchDTO> gamePlan)
        {
            await _hubContext.Clients.Client(connection).SendAsync(nameof(SendGamePlan), gamePlan);
        }
        protected override async Task SendGame(string connection, Connect4GameDTO game)
        {
            await _hubContext.Clients.Client(connection).SendAsync(nameof(SendGame), game);
        }
        protected override async Task SendBestList(string connection, IEnumerable<GameResult> bestlist)
        {
            await _hubContext.Clients.Client(connection).SendAsync(nameof(SendBestList), bestlist);
        }
        protected override async Task YouRequestedMatch(string connection, string playerId)
        {
            await _hubContext.Clients.Client(connection).SendAsync(nameof(YouRequestedMatch), playerId);
        }
        protected override async Task YouRejectedMatch(string connection, string playerId)
        {
            await _hubContext.Clients.Client(connection).SendAsync(nameof(YouRejectedMatch), playerId);
        }
        protected override async Task OpponentConfirmedGameStart(string connection)
        {
            await _hubContext.Clients.Client(connection).SendAsync(nameof(OpponentConfirmedGameStart));
        }
        protected override async Task GameStartConfirmed(string connection)
        {
            await _hubContext.Clients.Client(connection).SendAsync(nameof(GameStartConfirmed));
        }
        protected override async Task YouConfirmedGameStart(string connection)
        {
            await _hubContext.Clients.Client(connection).SendAsync(nameof(YouConfirmedGameStart));
        }
        protected override async Task SendHint(string connection, int hint)
        {
            await _hubContext.Clients.Client(connection).SendAsync(nameof(SendHint), hint);
        }

        private readonly IHubContext<THub> _hubContext;
    }
}

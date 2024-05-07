using backend.communication.DOTs;
using backend.Data;
using backend.game;
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
            await _hubContext.Clients.Client(connection).SendAsync("PlayerConnected", onlinePlayer);
        }
        protected override async Task PlayerDisconnected(string connection, string playerId)
        {
            await _hubContext.Clients.Client(connection).SendAsync("PlayerDisconnected", playerId);
        }
        protected override async Task PlayerRequestedMatch(string connection, string playerId)
        {
            await _hubContext.Clients.Client(connection).SendAsync("PlayerRequestedMatch", playerId);
        }
        protected override async Task PlayerRejectedMatch(string connection, string playerId)
        {
            await _hubContext.Clients.Client(connection).SendAsync("PlayerRejectedMatch", playerId);
        }
        protected override async Task Matched(string connection, MatchDTO match)
        {
            await _hubContext.Clients.Client(connection).SendAsync("Matched", match);
        }
        protected override async Task MatchingEnded(string connection, string matchId)
        {
            await _hubContext.Clients.Client(connection).SendAsync("MatchingEnded", matchId);
        }
        protected override async Task MovePlayed(string connection, string playerId, FieldDTO field)
        {
            await _hubContext.Clients.Client(connection).SendAsync("MovePlayed", playerId, field);
        }
        protected override async Task GameStarted(string connection, Connect4GameDTO connect4Game)
        {
            await _hubContext.Clients.Client(connection).SendAsync("GameStarted", connect4Game);
        }
        protected override async Task GameEnded(string connection, GameResultDTO gameResult)
        {
            await _hubContext.Clients.Client(connection).SendAsync("GameEnded", gameResult);
        }
        protected override async Task SendUserData(string connection, PlayerIdentityDTO userData)
        {
            await _hubContext.Clients.Client(connection).SendAsync("SendUserData", userData);
        }
        protected override async Task SendOnlinePlayers(string connection, IEnumerable<OnlinePlayerDTO> onlinePlayers)
        {
            await _hubContext.Clients.Client(connection).SendAsync("SendOnlinePlayers", onlinePlayers);
        }
        protected override async Task SendGamePlan(string connection, IEnumerable<MatchDTO> gamePlan)
        {
            await _hubContext.Clients.Client(connection).SendAsync("SendGamePlan", gamePlan);
        }
        protected override async Task SendGame(string connection, Connect4GameDTO game)
        {
            await _hubContext.Clients.Client(connection).SendAsync("SendGame", game);
        }
        protected override async Task YouRequestedMatch(string connection, string playerId)
        {
            await _hubContext.Clients.Client(connection).SendAsync("YouRequestedMatch", playerId);
        }
        protected override async Task YouRejectedMatch(string connection, string playerId)
        {
            await _hubContext.Clients.Client(connection).SendAsync("YouRejectedMatch", playerId);
        }

        private readonly IHubContext<THub> _hubContext;
    }
}

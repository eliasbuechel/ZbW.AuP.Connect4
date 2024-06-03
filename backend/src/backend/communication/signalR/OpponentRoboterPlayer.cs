using backend.communication.DOTs;
using backend.game;
using backend.game.entities;
using backend.services;
using Microsoft.AspNetCore.SignalR;

namespace backend.communication.signalR
{
    internal class OpponentRoboterPlayer : ToPlayerHub<OpponentRoboterPlayerHub>
    {
        public OpponentRoboterPlayer(string playerId, string username, GameManager gameManager, IHubContext<OpponentRoboterPlayerHub> hubContext) : base(playerId, username, gameManager, hubContext)
        { }

        protected override async Task PlayerRequestedMatch(string connection, string playerId)
        {
            await _hubContext.Clients.Client(connection).SendAsync(nameof(RequestMatch));
        }
        protected override async Task Matched(string connection, MatchDTO match)
        {
            await _hubContext.Clients.Client(connection).SendAsync(nameof(AcceptMatch));
        }
        protected override async Task PlayerRejectedMatch(string connection, string playerId)
        {
            await _hubContext.Clients.Client(connection).SendAsync(nameof(RejectMatch));
        }
        protected override async Task ConfirmedGameStart(string connection, string playerId)
        {
            await _hubContext.Clients.Client(connection).SendAsync(nameof(ConfirmGameStart));
        }
        protected override async Task MovePlayed(string connection, string playerId, FieldDTO field)
        {
            await _hubContext.Clients.Client(connection).SendAsync(nameof(PlayMove), field.Column);
        }
        protected override async Task GameEnded(string connection, GameResultDTO gameResult)
        {
            await _hubContext.Clients.Client(connection).SendAsync(nameof(QuitGame));
        }
    }
}

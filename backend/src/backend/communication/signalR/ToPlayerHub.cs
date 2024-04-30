using backend.communication.DOTs;
using backend.Data;
using backend.game;
using backend.services;
using Microsoft.AspNetCore.SignalR;

namespace backend.communication.signalR
{
    internal class ToPlayerHub<THub> : Player where THub : Hub
    {
        public ToPlayerHub(PlayerIdentity identity, GameManager gameManager, IHubContext<THub> hubContext) : base(identity, gameManager)
        {
            _hubContext = hubContext;
        }

        public override async void PlayerConnected(IPlayer player)
        {
            OnlinePlayerDTO onlinePlayer = new OnlinePlayerDTO(player, this);
            foreach (string connection in Connections)
                await _hubContext.Clients.Client(connection).SendAsync("player-connected", onlinePlayer);
        }
        public override async void PlayerDisconnected(IPlayer player)
        {
            foreach (string connection in Connections)
                await _hubContext.Clients.Client(connection).SendAsync("player-disconnected", player.Id);
        }
        public override async void RequestedMatch(IPlayer player)
        {
            foreach (string connection in Connections)
                await _hubContext.Clients.Client(connection).SendAsync("player-requested-match", player.Id);
        }
        public override async void RejectedMatch(IPlayer player)
        {
            foreach (string connection in Connections)
                await _hubContext.Clients.Client(connection).SendAsync("player-rejected-match", player.Id);
        }
        public override async void Matched(Match match)
        {
            MatchDTO matchDTO = new MatchDTO(match);

            foreach (string connection in Connections)
                await _hubContext.Clients.Client(connection).SendAsync("matched", matchDTO);
        }
        public override async void MovePlayed(int column)
        {
            foreach (string connection in Connections)
                await _hubContext.Clients.Client(connection).SendAsync("move-played", column);
        }
        public override async void GameStarted(Connect4Game connect4Game)
        {
            Connect4GameDTO connect4GameDTO = new Connect4GameDTO(connect4Game);

            foreach (string connection in Connections)
                await _hubContext.Clients.Client(connection).SendAsync("game-started", connect4GameDTO);
        }
        //public override async void GameEnded()
        //{
        //    foreach (string connection in Connections)
        //        await _hubContext.Clients.Client(connection).SendAsync("game-ended");
        //}
        public override async void GameEnded(GameResult gameResult)
        {
            GameResultDTO gameResultDTO = new GameResultDTO(gameResult);

            foreach (string connection in Connections)
                await _hubContext.Clients.Client(connection).SendAsync("game-ended", gameResultDTO);
        }

        private readonly IHubContext<THub> _hubContext;
    }
}

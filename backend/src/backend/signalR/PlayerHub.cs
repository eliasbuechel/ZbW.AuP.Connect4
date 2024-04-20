using backend.game;
using backend.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;

namespace backend.signalR
{
    [Authorize]
    internal class PlayerHub : Hub, IPlayerApi 
    {
        public PlayerHub(PlayerManager playerManager, GameManager gameManager)
        {
            _playerManager = playerManager;
            _gameManager = gameManager;
        }

        public void MakeMove(int column)
        {
            IPlayer player = GetPlayer();
            player.MakeMove(column);
        }

        public async void GetPlayers()
        {
            IEnumerable<IPlayer> players = _playerManager.Players;
            string jsonPlayers = JsonSerializer.Serialize(players);
            await Clients.Caller.SendAsync("get-players", jsonPlayers);
        }

        private IPlayer GetPlayer()
        {
            ClaimsPrincipal? claimsPrincipal = Context.User;
            Debug.Assert(claimsPrincipal != null);
            string? userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            Debug.Assert(userId != null);

            return _playerManager.GetPlayerByUserId(userId);
        }
        public void ConfirmGameStart()
        {
            IPlayer player = GetPlayer();
            player.ConfirmGameStart();
        }
        public override Task OnConnectedAsync()
        {
            IPlayer player = GetPlayer();
            _gameManager.AddPlayer(player);
            return Task.CompletedTask;
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            IPlayer player = GetPlayer();
            _gameManager.RemovePlayer(player);
            return Task.CompletedTask;
        }

        private readonly PlayerManager _playerManager;
        private readonly GameManager _gameManager;
    }
}

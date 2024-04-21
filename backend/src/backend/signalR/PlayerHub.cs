using backend.database;
using backend.game;
using backend.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;

namespace backend.signalR
{
    [Authorize]
    internal class PlayerHub : Hub, IPlayerApi 
    {
        public PlayerHub(services.PlayerManager playerManager, GameManager gameManager, UserManager<PlayerIdentity> userManager)
        {
            _playerManager = playerManager;
            _gameManager = gameManager;
            _userManager = userManager;
        }

        public void MakeMove(int column)
        {
            PlayerIdentity identity = GetIdentity();
            IPlayer player = _playerManager.GetPlayer(identity);
            player.MakeMove(column);
        }

        public async void GetPlayers()
        {
            IEnumerable<IPlayer> players = _playerManager.Players;
            string jsonPlayers = JsonSerializer.Serialize(players);
            await Clients.Caller.SendAsync("get-players", jsonPlayers);
        }

        public void ConfirmGameStart()
        {
            //IPlayer player = GetPlayer();
            //player.ConfirmGameStart();
        }
        public override Task OnConnectedAsync()
        {
            PlayerIdentity identity = GetIdentity();
            _playerManager.OnConnected(identity);
            return Task.CompletedTask;
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            PlayerIdentity identity = GetIdentity();
            _playerManager.OnDisconnected(identity);
            return Task.CompletedTask;
        }


        private PlayerIdentity GetIdentity()
        {
            ClaimsPrincipal? claimsPrincipal = Context.User;
            Debug.Assert(claimsPrincipal != null);

            string? userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            Debug.Assert(userId != null);

            PlayerIdentity? identity = _userManager.FindByIdAsync(userId).Result;
            Debug.Assert(identity != null);

            return identity;
        }

        private readonly services.PlayerManager _playerManager;
        private readonly GameManager _gameManager;
        private readonly UserManager<PlayerIdentity> _userManager;
    }
}

using backend.database;
using backend.game;
using backend.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
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
            IEnumerable<OnlinePlayerDTO> onlinePlayersDTO = players.Select(p => new OnlinePlayerDTO(p));
            string jsonPlayers = JsonSerializer.Serialize(onlinePlayersDTO);
            await Clients.Caller.SendAsync("get-online-players", onlinePlayersDTO);
        }

        public void ConfirmGameStart()
        {
            //IPlayer player = GetPlayer();
            //player.ConfirmGameStart();
        }
        public override Task OnConnectedAsync()
        {
            PlayerIdentity identity = GetIdentity();
            _playerManager.OnPlayerConnected(identity);
            return Task.CompletedTask;
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            PlayerIdentity identity = GetIdentity();
            _playerManager.OnPlayerDisconnected(identity);
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

    internal class OnlinePlayerDTO
    {
        public OnlinePlayerDTO(IPlayer player)
        {
            id = player.Id;
            string? username = player.Username;
            Debug.Assert(username != null);
            this.username = username;
            requestedMatch = false;
        }
        public string id { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public bool requestedMatch { get; set; }
    }
}

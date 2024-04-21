using backend.database;
using backend.game;
using backend.signalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Collections;
using System.Diagnostics;
using System.Numerics;

namespace backend.services
{
    internal class PlayerManager
    {
        public PlayerManager(GameManager gameManager, IHubContext<PlayerHub> playerHubContext)
        {

            _gameManager = gameManager;
            _playerHubContext = playerHubContext;
        }

        public IEnumerable<IPlayer> Players => _identityToPlayerMap.Values.ToList();

        public void OnPlayerConnected(PlayerIdentity identity)
        {
            if (_identityToPlayerMap.ContainsKey(identity.Id))
            {
                _playerConnectionCounterMap[identity.Id]++;
                return;
            }

            _playerConnectionCounterMap.Add(identity.Id, 1);

            IPlayer player = new Player(identity, _gameManager, _playerHubContext);
            _identityToPlayerMap.Add(identity.Id, player);
            _playerHubContext.Clients.AllExcept(identity.Id).SendAsync("player-connected", new OnlinePlayerDTO(player));
        }
        public void OnPlayerDisconnected(PlayerIdentity identity)
        {
            _playerConnectionCounterMap[identity.Id]--;

            if (_playerConnectionCounterMap[identity.Id] > 0)
                return;

            _playerConnectionCounterMap.Remove(identity.Id);
            _identityToPlayerMap.Remove(identity.Id);
            _playerHubContext.Clients.AllExcept(identity.Id).SendAsync("player-disconnected", identity.Id);
        }

        public IPlayer GetPlayer(PlayerIdentity identity)
        {
            IPlayer? player = _identityToPlayerMap.GetValueOrDefault(identity.Id);
            Debug.Assert(player != null);
            return player;
        }

        private readonly GameManager _gameManager;
        private readonly IHubContext<PlayerHub> _playerHubContext;
        private readonly Dictionary<string, IPlayer> _identityToPlayerMap = new Dictionary<string, IPlayer>();
        private readonly Dictionary<string, int> _playerConnectionCounterMap = new Dictionary<string, int>();
    }
}

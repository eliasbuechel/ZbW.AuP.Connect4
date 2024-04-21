using backend.database;
using backend.game;
using backend.signalR;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace backend.services
{
    internal class PlayerManager
    {
        public PlayerManager(GameManager gameManager, IHubContext<SignalRPlayerHub> playerHubContext)
        {

            _gameManager = gameManager;
            _playerHubContext = playerHubContext;
        }

        public IEnumerable<IPlayer> OnlinePlayers => _onlinePlayers;

        public void OnPlayerConnected(IPlayer player)
        {
            if (_onlinePlayers.Contains(player))
            {
                _playerConnectionCounterMap[player.Id]++;
                return;
            }

            _playerConnectionCounterMap.Add(player.Id, 1);

            foreach (var onlinePlayer in _onlinePlayers)
                onlinePlayer.PlayerConnected(player);

            _onlinePlayers.Add(player);
        }
        public void OnPlayerDisconnected(IPlayer player)
        {
            _playerConnectionCounterMap[player.Id]--;

            if (_playerConnectionCounterMap[player.Id] > 0)
                return;

            _playerConnectionCounterMap.Remove(player.Id);
            _onlinePlayers.Remove(player);

            foreach (var onlinePlayer in _onlinePlayers)
                onlinePlayer.PlayerDisconnected(player);
        }

        public IPlayer GetPlayer(PlayerIdentity identity)
        {
            return GetPlayer(identity.Id);
        }
        public IPlayer GetPlayer(string playerId)
        {
            IPlayer? player = _onlinePlayers.FirstOrDefault(p => p.Id == playerId);
            Debug.Assert(player != null);
            return player;
        }
        public IPlayer? GetPlayerOrDefault(PlayerIdentity identity)
        {
            return _onlinePlayers.FirstOrDefault(p => p.Id == identity.Id);
        }

        private readonly GameManager _gameManager;
        private readonly IHubContext<SignalRPlayerHub> _playerHubContext;

        private readonly ICollection<IPlayer> _onlinePlayers = new List<IPlayer>();
        private readonly Dictionary<string, int> _playerConnectionCounterMap = new Dictionary<string, int>();
    }
}

using backend.Data;
using backend.game;
using System.Diagnostics;

namespace backend.services
{
    internal class PlayerManager
    {
        public PlayerManager(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public IEnumerable<IPlayer> OnlinePlayers => _onlinePlayers;

        public void OnPlayerConnected(IPlayer player)
        {
            if (_playerConnectionCounterMap.ContainsKey(player.Id))
            {
                _playerConnectionCounterMap[player.Id]++;
                return;
            }

            _playerConnectionCounterMap.Add(player.Id, 1);

            foreach (var onlinePlayer in _onlinePlayers)
                onlinePlayer.PlayerConnected(player);

            _onlinePlayers.Add(player);
        }
        public async void OnPlayerDisconnected(IPlayer player)
        {
            _playerConnectionCounterMap[player.Id]--;

            if (_playerConnectionCounterMap[player.Id] > 0)
                return;

            await Task.Run(async () =>
            {
                await Task.Delay(5000);

                int activeConnectionCount;

                if (!_playerConnectionCounterMap.TryGetValue(player.Id, out activeConnectionCount))
                    return;

                if (activeConnectionCount > 0)
                    return;

                _playerConnectionCounterMap.Remove(player.Id);
                _onlinePlayers.Remove(player);
                _gameManager.PlayerQuit(player);

                foreach (var onlinePlayer in _onlinePlayers)
                    onlinePlayer.PlayerDisconnected(player);
            });
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

        private readonly ICollection<IPlayer> _onlinePlayers = new List<IPlayer>();
        private readonly Dictionary<string, int> _playerConnectionCounterMap = new Dictionary<string, int>();
    }
}

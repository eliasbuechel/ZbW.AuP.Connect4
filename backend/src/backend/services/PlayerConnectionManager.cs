using backend.database;
using backend.game;
using System.Diagnostics;

namespace backend.services
{
    internal class PlayerConnectionManager : IOnlinePlayerProvider
    {
        private static int instanceCount = 0;
        public PlayerConnectionManager()
        {
            instanceCount++;
            Debug.Assert(instanceCount < 2); // only one instance allowed
        }

        public IEnumerable<IPlayer> OnlinePlayers
        {
            get
            {
                lock (_onlinePlayersLock)
                {
                    return _onlinePlayers.ToArray();
                }
            }
        }

        public void ConnectPlayer(IPlayer player)
        {
            lock (_playerConnectionCounterMapLock)
            {
                if (_playerConnectionCounterMap.ContainsKey(player.Id))
                {
                    _playerConnectionCounterMap[player.Id]++;
                    return;
                }

                _playerConnectionCounterMap.Add(player.Id, 1);
            }

            lock(_onlinePlayersLock)
            {
                foreach (var onlinePlayer in _onlinePlayers)
                    onlinePlayer.PlayerConnected(player);

                _onlinePlayers.Add(player);
            }
        }
        public async void DisconnectPlayer(IPlayer player, Action<IPlayer>? playerQuitCallback)
        {
            lock (_playerConnectionCounterMapLock)
            { 
                _playerConnectionCounterMap[player.Id]--;

                if (_playerConnectionCounterMap[player.Id] > 0)
                    return;
            }

            await Task.Run(async () =>
            {
                await Task.Delay(5000);

                int activeConnectionCount;

                lock (_playerConnectionCounterMapLock)
                {
                    if (!_playerConnectionCounterMap.TryGetValue(player.Id, out activeConnectionCount))
                        return;

                    if (activeConnectionCount > 0)
                        return;

                    _playerConnectionCounterMap.Remove(player.Id);
                }

                lock (_onlinePlayersLock)
                {
                    _onlinePlayers.Remove(player);
                    playerQuitCallback?.Invoke(player);

                    foreach (var onlinePlayer in _onlinePlayers)
                        onlinePlayer.PlayerDisconnected(player);
                }
            });
        }

        public IPlayer GetPlayer(PlayerIdentity identity)
        {
            return GetPlayer(identity.Id);
        }
        public IPlayer GetPlayer(string playerId)
        {
            lock(_onlinePlayersLock)
            {
                IPlayer? player = _onlinePlayers.FirstOrDefault(p => p.Id == playerId);
                Debug.Assert(player != null);
                return player;
            }
        }
        public IPlayer? GetPlayerOrDefault(PlayerIdentity identity)
        {
            lock(_onlinePlayersLock)
            {
                return _onlinePlayers.FirstOrDefault(p => p.Id == identity.Id);
            }
        }

        private readonly object _onlinePlayersLock = new object();
        private readonly object _playerConnectionCounterMapLock = new object();
        private readonly ICollection<IPlayer> _onlinePlayers = new List<IPlayer>();
        private readonly Dictionary<string, int> _playerConnectionCounterMap = new Dictionary<string, int>();
    }
}

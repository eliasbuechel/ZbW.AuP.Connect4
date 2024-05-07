using backend.communication.signalR;
using backend.Data;
using backend.game;
using System.Diagnostics;
using System.Security.Principal;

namespace backend.services
{
    internal class PlayerConnectionManager : IOnlinePlayerProvider
    {
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

        public IPlayer GetOnlinePlayer(string playerId)
        {
            lock(_onlinePlayersLock)
            {
                IPlayer? player = _onlinePlayers.FirstOrDefault(p => p.Id == playerId);
                Debug.Assert(player != null);
                return player;
            }
        }
        public IPlayer? GetOnlinePlayerOrDefault(string playerId)
        {
            lock (_onlinePlayersLock)
            {
                return _onlinePlayers.FirstOrDefault(p => p.Id == playerId);
            }
        }

        private readonly object _onlinePlayersLock = new object();
        private readonly object _playerConnectionCounterMapLock = new object();
        private readonly ICollection<IPlayer> _onlinePlayers = new List<IPlayer>();
        private readonly Dictionary<string, int> _playerConnectionCounterMap = new Dictionary<string, int>();
    }
}

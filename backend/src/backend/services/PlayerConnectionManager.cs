using backend.communication.signalR;
using backend.Data;
using backend.game;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

namespace backend.services
{
    internal class PlayerConnectionManager : IOnlinePlayerProvider
    {
        public IEnumerable<IPlayer> OnlinePlayers => _onlinePlayers.ToArray();

        public void ConnectPlayer(IPlayer player)
        {
            if (_playerConnectionCounterMap.ContainsKey(player.Id))
            {
                _playerConnectionCounterMap[player.Id]++;
                return;
            }

            _playerConnectionCounterMap.TryAdd(player.Id, 1);

            foreach (var onlinePlayer in _onlinePlayers)
                if (onlinePlayer != player)
                    onlinePlayer.PlayerConnected(player);
        }
        public async void DisconnectPlayer(IPlayer player, Action<IPlayer>? playerQuitCallback)
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

                int value;
                _playerConnectionCounterMap.Remove(player.Id, out value);
                playerQuitCallback?.Invoke(player);

                foreach (var onlinePlayer in _onlinePlayers)
                    onlinePlayer.PlayerDisconnected(player);
            });
        }

        public IPlayer GetOnlinePlayerAsync(string playerId)
        {
            IPlayer? player = _onlinePlayers.FirstOrDefault(p => p.Id == playerId);
            Debug.Assert(player != null);
            return player;
        }
        public IPlayer? GetOnlinePlayerOrDefault(string playerId)
        {
            return _onlinePlayers.FirstOrDefault(p => p.Id == playerId);
        }
        public IPlayer GetOrCreatePlayer(PlayerIdentity playerIdentity, Func<PlayerIdentity, ToPlayerHub<WebPlayerHub>> createPlayer)
        {
            IPlayer? player = GetOnlinePlayerOrDefault(playerIdentity.Id);
            if (player == null)
            {
                player = createPlayer(playerIdentity);
                _onlinePlayers.Add(player);
            }

            return player;
        }

        private readonly ConcurrentBag<IPlayer> _onlinePlayers = new ConcurrentBag<IPlayer>();
        private readonly ConcurrentDictionary<string, int> _playerConnectionCounterMap = new ConcurrentDictionary<string, int>();
    }
}
using backend.communication.signalR;
using backend.game;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace backend.services
{
    internal abstract class PlayerManager<TPlayer, TIdentitfication> : IPlayerProvider<TPlayer, TIdentitfication> where TPlayer : IPlayer
    {
        public event Action<TPlayer>? OnPlayerConnected;
        public event Action<TPlayer>? OnPlayerDisconnected;

        public IEnumerable<TPlayer> ConnectedPlayers => _connectedPlayers.ToArray();

        public TPlayer GetConnectedPlayer(string playerId)
        {
            TPlayer? player = _connectedPlayers.FirstOrDefault(p => p.Id == playerId);
            Debug.Assert(player != null);
            return player;
        }
        public TPlayer? GetConnectedPlayerOrDefault(string playerId)
        {
            return _connectedPlayers.FirstOrDefault(p => p.Id == playerId);
        }
        public TPlayer ConnectPlayer(TIdentitfication identification, Func<TIdentitfication, TPlayer> createPlayer)
        {
            TPlayer player = GetOrCreatePlayer(identification, createPlayer);
            ConnectPlayer(player);
            return player;
        }
        public TPlayer DisconnectPlayer(TIdentitfication identitfication)
        {
            TPlayer player = GetConnectedPlayerByIdentification(identitfication);

            _playerConnectionCounterMap[player.Id]--;

            if (_playerConnectionCounterMap[player.Id] > 0)
                return player;

            Thread thread = new Thread(async () =>
            {
                await Task.Delay(5000);

                int activeConnectionCount;

                if (!_playerConnectionCounterMap.TryGetValue(player.Id, out activeConnectionCount))
                    return;

                if (activeConnectionCount > 0)
                    return;

                int value;
                _playerConnectionCounterMap.Remove(player.Id, out value);

                PlayerDisconnected(player);
            });
            thread.Start();

            return player;
        }

        public abstract TPlayer GetConnectedPlayerByIdentification(TIdentitfication identitfication);
        public abstract TPlayer GetOrCreatePlayer(TIdentitfication identification, Func<TIdentitfication, TPlayer> createPlayer);

        private void PlayerDisconnected(TPlayer player)
        {
            foreach (var onlinePlayer in _connectedPlayers)
                onlinePlayer.PlayerDisconnected(player);

            OnPlayerDisconnected?.Invoke(player);
        }
        private void PlayerConnected(TPlayer player)
        {
            foreach (var connectedPlayer in _connectedPlayers)
                if (connectedPlayer.Id != player.Id)
                    connectedPlayer.PlayerConnected(player);

            OnPlayerConnected?.Invoke(player);
        }

        private void ConnectPlayer(TPlayer player)
        {
            if (_playerConnectionCounterMap.ContainsKey(player.Id))
            {
                _playerConnectionCounterMap[player.Id]++;
                return;
            }

            if (_playerConnectionCounterMap.TryAdd(player.Id, 1))
                PlayerConnected(player);
        }

        protected readonly ConcurrentBag<TPlayer> _connectedPlayers = new ConcurrentBag<TPlayer>();
        protected readonly ConcurrentDictionary<string, int> _playerConnectionCounterMap = new ConcurrentDictionary<string, int>();
    }
}
using backend.game;
using backend.Infrastructure;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace backend.services.player
{
    internal abstract class PlayerConnectionManager<TPlayer, TIdentitfication> : DisposingObject where TPlayer : Player where TIdentitfication : notnull
    {
        public event Action<TPlayer>? OnPlayerConnected;
        public event Action<TPlayer>? OnPlayerDisconnected;

        public IEnumerable<TPlayer> ConnectedPlayers => _connectedPlayersAndIdentification.Select(x => x.Item1).ToArray();

        public TPlayer GetConnectedPlayer(string playerId)
        {
            TPlayer? player = _connectedPlayersAndIdentification.Select(x => x.Item1).FirstOrDefault(p => p.Id == playerId);
            Debug.Assert(player != null);
            return player;
        }
        public TPlayer GetConnectedPlayer(TIdentitfication identitfication)
        {
            return _connectedPlayersAndIdentification.Where(x => x.Item2.Equals(identitfication)).Select(x => x.Item1).First();
        }
        public TPlayer? GetConnectedPlayerOrDefault(string playerId)
        {
            return _connectedPlayersAndIdentification.Select(x => x.Item1).FirstOrDefault(p => p.Id == playerId);
        }
        public TPlayer? GetConnectedPlayerOrDefault(TIdentitfication identitfication)
        {
            return _connectedPlayersAndIdentification.Where(x => x.Item2.Equals(identitfication)).Select(x => x.Item1).FirstOrDefault();
        }
        public void ConnectPlayer(TIdentitfication identification, string connectionId)
        {
            TPlayer player = GetOrCreatePlayer(identification);
            ConnectPlayer(player, identification, connectionId);
        }
        public void DisconnectPlayer(TIdentitfication identitfication, string connectionId)
        {
            TPlayer player = GetConnectedPlayer(identitfication);
            DisconnectPlayer(player, identitfication, connectionId);
        }
        public void ForeachConnectedPlayerConnection(Action<string> action)
        {
            foreach (var p in _connectedPlayersAndIdentification)
            {
                foreach (var c in _playerConnections[p.Item2])
                    action(c);
            }
        }
        public void ForeachConnectionOfPlayer(TPlayer player, Action<string> action)
        {
            foreach (var pi in _connectedPlayersAndIdentification)
            {
                if (pi.Item1 != player)
                    continue;

                foreach (var c in _playerConnections[pi.Item2])
                    action(c);
            }
        }

        protected TPlayer DisconnectPlayer(TPlayer player, TIdentitfication identitfication, string connectionId)
        {
            for (int i = 0; i < _playerConnections[identitfication].Count; i++)
            {
                if (_playerConnections[identitfication][i] == connectionId)
                {
                    _playerConnections[identitfication].Remove(_playerConnections[identitfication][i]);
                    break;
                }
            }

            if (_playerConnections[identitfication].Count > 0)
                return player;

            Thread thread = new Thread(async () =>
            {
                await Task.Delay(5000);

                List<string>? connections;

                if (!_playerConnections.TryGetValue(identitfication, out connections))
                    return;

                if (connections.Count > 0)
                    return;

                List<string>? value;
                _playerConnections.Remove(identitfication, out value);

                PlayerDisconnected(player);
            });
            thread.Start();

            return player;
        }
        protected void ForeachConnectedPlayerConnectionExcept(TPlayer exceptingPlayer, Action<string> action)
        {
            foreach (var p in _connectedPlayersAndIdentification)
            {
                if (p.Item1 == exceptingPlayer)
                    continue;

                foreach (var c in _playerConnections[p.Item2])
                    action(c);
            }
        }

        protected virtual void PlayerConnected(TPlayer player)
        {
            OnPlayerConnected?.Invoke(player);
        }
        protected virtual void PlayerDisconnected(TPlayer player)
        {
            OnPlayerDisconnected?.Invoke(player);
        }
        protected abstract TPlayer GetOrCreatePlayer(TIdentitfication identification);
        protected abstract bool IdentificationsAreEqal(TIdentitfication identification1, TIdentitfication identification2);

        private void ConnectPlayer(TPlayer player, TIdentitfication identification, string connectionId)
        {
            if (_playerConnections.ContainsKey(identification))
            {
                _playerConnections[identification].Add(connectionId);
                return;
            }

            if (_playerConnections.TryAdd(identification, new List<string>([connectionId])))
                PlayerConnected(player);
        }

        protected readonly ConcurrentBag<Tuple<TPlayer, TIdentitfication>> _connectedPlayersAndIdentification = new ConcurrentBag<Tuple<TPlayer, TIdentitfication>>();
        protected readonly ConcurrentDictionary<TIdentitfication, List<string>> _playerConnections = new ConcurrentDictionary<TIdentitfication, List<string>>();
    }
}
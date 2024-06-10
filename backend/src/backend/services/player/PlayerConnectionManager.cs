using backend.game;
using backend.Infrastructure;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;

namespace backend.services.player
{
    internal abstract class PlayerConnectionManager<TPlayer, TIdentitfication> : DisposingObject where TPlayer : Player where TIdentitfication : notnull
    {
        public event Action<TPlayer>? OnPlayerConnected;
        public event Action<TPlayer>? OnPlayerDisconnected;

        public IEnumerable<TPlayer> ConnectedPlayers => _connections.Select(x => x.Player).ToArray();

        public TPlayer GetConnectedPlayer(string playerId)
        {
            TPlayer? player = _connections.Select(x => x.Player).FirstOrDefault(p => p.Id == playerId);
            Debug.Assert(player != null);
            return player;
        }
        public TPlayer GetConnectedPlayerByIdentification(TIdentitfication identitfication)
        {
            return _connections.Where(x => x.Identification.Equals(identitfication)).Select(x => x.Player).First();
        }
        public TPlayer? GetConnectedPlayerOrDefault(string playerId)
        {
            return _connections.Select(x => x.Player).FirstOrDefault(p => p.Id == playerId);
        }
        public TPlayer? GetConnectedPlayerByIdentificationOrDefault(TIdentitfication identitfication)
        {
            return _connections.Where(x => x.Identification.Equals(identitfication)).Select(x => x.Player).FirstOrDefault();
        }
        public void ConnectPlayer(TIdentitfication identification, string connectionId)
        {
            TPlayer player = GetOrCreatePlayer(identification, connectionId);
            ConnectPlayer(player, identification, connectionId);
        }
        public void DisconnectPlayer(TIdentitfication identitfication, string connectionId)
        {
            TPlayer player = GetConnectedPlayerByIdentification(identitfication);
            DisconnectPlayer(player, identitfication, connectionId);
        }
        public void ForeachConnectedPlayerConnection(Action<string> action)
        {
            foreach (var c in _connections)
            {
                foreach (var connectionId in c.ConnectionIds)
                    action(connectionId);
            }
        }
        public void ForeachConnectedPlayerConnection(Func<TPlayer, bool> condition, Action<string> action)
        {
            foreach (var c in _connections)
            {
                if (!condition(c.Player))
                    continue;

                foreach (var connectionId in c.ConnectionIds)
                    action(connectionId);
            }
        }
        public void ForeachConnectionOfPlayer(TPlayer player, Action<string> action)
        {
            foreach (var c in _connections)
            {
                if (c.Player != player)
                    continue;

                foreach (var connectionId in c.ConnectionIds)
                    action(connectionId);
            }
        }

        protected void DisconnectPlayer(TPlayer player, TIdentitfication identitfication, string connectionId)
        {
            PlayerConnection connection = _connections.Where(x => x.Identification.Equals(identitfication)).First();
            connection.ConnectionIds.Remove(connection.ConnectionIds.Where(x => x.Equals(connectionId)).First());

            if (connection.ConnectionIds.Count > 0)
                return;

            Thread thread = new Thread(async () =>
            {
                await Task.Delay(5000);

                PlayerConnection? connection = _connections.Where(x => x.Identification.Equals(identitfication)).FirstOrDefault();

                if (connection == null)
                    return;

                if (connection.ConnectionIds.Count > 0)
                    return;

                _connections = new ConcurrentBag<PlayerConnection>(_connections.Where(x => x.Equals(connection)));
                PlayerDisconnected(player);
            });
            thread.Start();
        }
        protected void ForeachConnectedPlayerConnectionExcept(TPlayer exceptingPlayer, Action<string> action)
        {
            foreach (var c in _connections)
            {
                if (c.Player.Equals(exceptingPlayer))
                    continue;

                foreach (var connectionId in c.ConnectionIds)
                    action(connectionId);
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
        protected abstract TPlayer GetOrCreatePlayer(TIdentitfication identification, string connectionId);

        private void ConnectPlayer(TPlayer player, TIdentitfication identification, string connectionId)
        {
            PlayerConnection? connection = _connections.Where(x => x.Identification.Equals(identification)).FirstOrDefault();

            if (connection != null)
            {
                connection.ConnectionIds.Add(connectionId);
                return;
            }

            _connections.Add(new PlayerConnection(player, identification, connectionId));
        }

        protected ConcurrentBag<PlayerConnection> _connections = new ConcurrentBag<PlayerConnection>();

        protected class PlayerConnection
        {
            public PlayerConnection(TPlayer player, TIdentitfication identification, string connectionId)
            {
                Player = player;
                Identification = identification;
                ConnectionIds = new List<string>() { connectionId };
            }

            public TPlayer Player { get; }
            public TIdentitfication Identification { get; }
            public List<string> ConnectionIds { get; }
        }
    }
}
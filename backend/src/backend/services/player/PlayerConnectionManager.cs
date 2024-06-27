using backend.game;
using backend.infrastructure;
using backend.utilities;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace backend.services.player
{
    internal abstract class PlayerConnectionManager<TPlayer, TIdentitfication> : DisposingObject where TPlayer : Player where TIdentitfication : notnull
    {
        public event Action<TPlayer>? OnPlayerConnected;
        public event Action<TPlayer>? OnPlayerDisconnected;

        public IEnumerable<TPlayer> ConnectedPlayers => _connections.Select(x => x.Player);

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
        public TPlayer GetConnectedPlayerByConnectionId(string connectionId)
        {
            return _connections
                .Where(x => x.ConnectionIds.Any(c => c.Equals(connectionId)))
                .Select(x => x.Player)
                .First();
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
            CreateOrConnectPlayer(identification, connectionId);
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
            PlayerConnection connection = _connections.First(x => x.Identification.Equals(identitfication));
            connection.ConnectionIds.Remove(connection.ConnectionIds.First(x => x.Equals(connectionId)));

            if (connection.ConnectionIds.Count > 0)
                return;

            connection.TimeoutCounter++;
            Thread thread = new(async () =>
            {
                await Task.Delay(DISCONECCTING_TIMOUT_TIME_IN_MS);

                PlayerConnection? connection = _connections.FirstOrDefault(x => x.Identification.Equals(identitfication));

                if (connection == null)
                    return;

                connection.TimeoutCounter--;
                if (connection.TimeoutCounter > 0)
                    return;

                if (connection.ConnectionIds.Count > 0)
                    return;

                _connections = new ConcurrentBag<PlayerConnection>(_connections.Where(x => !x.Equals(connection)));
                try
                {
                    PlayerDisconnected(player);
                }
                catch (InvalidPlayerRequestException)
                {
                    Logger.Log(LogLevel.Information, LogContext.CONNECTION_MANAGER, $"Not able to disconnect. Player: '{player.Username}' Identification: '{identitfication}' ConnectionId: '{connection}'");
                }
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
        protected abstract void CreateOrConnectPlayer(TIdentitfication identification, string connectionId);

        protected void ConnectPlayer(TPlayer player, TIdentitfication identification, string connectionId)
        {
            PlayerConnection? connection = _connections.FirstOrDefault(x => x.Identification.Equals(identification));

            if (connection != null)
            {
                connection.ConnectionIds.Add(connectionId);
                return;
            }

            _connections.Add(new PlayerConnection(player, identification, connectionId));
            PlayerConnected(player);
        }

        protected ConcurrentBag<PlayerConnection> _connections = [];

        protected class PlayerConnection(TPlayer player, TIdentitfication identification, string connectionId)
        {
            public TPlayer Player { get; } = player;
            public TIdentitfication Identification { get; } = identification;
            public List<string> ConnectionIds { get; } = [connectionId];
            public int TimeoutCounter { get; set; }

            public override bool Equals(object? obj)
            {
                return obj is PlayerConnectionManager<TPlayer, TIdentitfication>.PlayerConnection connection &&
                       EqualityComparer<TIdentitfication>.Default.Equals(Identification, connection.Identification);
            }
            public override int GetHashCode()
            {
                return HashCode.Combine(Identification);
            }
        }

        private const int DISCONECCTING_TIMOUT_TIME_IN_MS = 30000;
    }
}

using backend.Data;
using System.Diagnostics;

namespace backend.game
{
    internal abstract class Player : IPlayer
    {
        public Player(PlayerIdentity identity, GameManager gameManager, backend.services.PlayerManager playerManager)
        {
            Id = identity.Id;
            string? username = identity.UserName;
            Debug.Assert(username != null);
            Username = username;

            _gameManager = gameManager;
            _playerManager = playerManager;
        }

        public bool HasConfirmedGameStart { get; private set; }

        public string Id { get; }

        public string Username { get; }
        public IEnumerable<string> Connections => _connections;

        public event Action<IPlayer, int>? OnMovePlayed;
        public event Action<IPlayer, IPlayer>? OnMatch;

        public IEnumerable<IPlayer> GetOnlinePlayers()
        {
            return _playerManager.OnlinePlayers.Where(p => p.Id != Id);
        }
        public void Connected(string connectionId)
        {
            _connections.Add(connectionId);
            _playerManager.OnPlayerConnected(this);
        }
        public void Disconnected(string onnectionId)
        {
            _connections.Remove(onnectionId);
            _playerManager.OnPlayerDisconnected(this);
        }
        public void RequestMatch(IPlayer player)
        {
            _gameManager.RequestMatch(this, player);
        }
        //public void MakeMove(int column)
        //{
        //    _gameManager.MakeMove(this, column);
        //}
        public void ConfirmGameStart()
        {
            Debug.Assert(!HasConfirmedGameStart);
            HasConfirmedGameStart = true;
        }

        public abstract void RequestedMatch(IPlayer player);
        public abstract void RejectedMatch(IPlayer player);
        public abstract void PlayerConnected(IPlayer player);
        public abstract void PlayerDisconnected(IPlayer player);

        public bool HasRequestedMatch(IPlayer you)
        {
            return _gameManager.HasRequestedMatch(this, you);
        }

        public bool HasMatched(IPlayer player)
        {
            return _gameManager.HasMatched(this, player);
        }

        public void AcceptMatch(IPlayer player)
        {
            _gameManager.AcceptMatch(this, player);
        }

        public abstract void Matched(IPlayer requester);

        public void RejectMatch(IPlayer player)
        {
            _gameManager.RejectMatch(this, player);
        }

        private readonly GameManager _gameManager;
        private readonly backend.services.PlayerManager _playerManager;
        private readonly ICollection<string> _connections = new List<string>();
    }
}

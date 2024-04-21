
using backend.database;
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

        public event Action<IPlayer, int>? OnMovePlayed;
        public event Action<IPlayer, IPlayer>? OnMatch;

        public IEnumerable<IPlayer> GetOnlinePlayers()
        {
            return _playerManager.OnlinePlayers.Where(p => p.Id != Id);
        }
        public void Connected()
        {
            _playerManager.OnPlayerConnected(this);
        }
        public void Disconnected()
        {
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
        public abstract void DeclineMatch(IPlayer player);
        public abstract void PlayerConnected(IPlayer player);
        public abstract void PlayerDisconnected(IPlayer player);


        private readonly GameManager _gameManager;
        private readonly backend.services.PlayerManager _playerManager;
    }
}

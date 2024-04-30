using backend.database;
using backend.services;
using System.Diagnostics;

namespace backend.game
{
    internal abstract class Player : IPlayer
    {
        public Player(PlayerIdentity identity, GameManager gameManager)
        {
            Id = identity.Id;
            string? username = identity.UserName;
            Debug.Assert(username != null);
            Username = username;

            _gameManager = gameManager;
        }

        public bool HasConfirmedGameStart { get; private set; }

        public string Id { get; }

        public string Username { get; }
        public IEnumerable<string> Connections => _connections;

        public event Action<IPlayer, int>? OnMovePlayed;
        public event Action<IPlayer, IPlayer>? OnMatch;

        public IEnumerable<IPlayer> GetOnlinePlayers()
        {
            return _gameManager.GetOnlinePlayersExcept(Id);
        }
        public void Connected(string connectionId)
        {
            _connections.Add(connectionId);
            _gameManager.ConnectPlayer(this);
        }
        public void Disconnected(string onnectionId)
        {
            _connections.Remove(onnectionId);
            _gameManager.DisconnectPlayer(this);
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

        public abstract void Matched(Match match);

        public void RejectMatch(IPlayer player)
        {
            _gameManager.RejectMatch(this, player);
        }

        public IEnumerable<Match> GetGamePlan()
        {
            return _gameManager.GetGamePlan();
        }

        public abstract void MovePlayed(int column);

        public void PlayMove(int column)
        {
            _gameManager.PlayMove(this, column);
        }

        public abstract void GameStarted(Connect4Game connect4Game);

        public Connect4Game GetCurrentGameState()
        {
            return _gameManager.GetCurrentGameState();
        }

        public void QuitGame()
        {
            _gameManager.QuitGame(this);
        }

        public abstract void GameEnded(GameResult gameResult);

        public bool HasGameStarted()
        {
            return _gameManager.HasGameStarted(this);
        }

        private readonly GameManager _gameManager;
        private readonly ICollection<string> _connections = new List<string>();
    }
}

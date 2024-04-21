
using backend.database;
using backend.signalR;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace backend.game
{
    internal class Player : IPlayer
    {
        public Player(PlayerIdentity identity, GameManager gameManager, IHubContext<PlayerHub> playerHubContext)
        {
            Id = identity.Id;
            string? username = identity.UserName;
            Debug.Assert(username != null);
            Username = username;

            _gameManager = gameManager;
            _playerHubContext = playerHubContext;
        }

        public bool HasConfirmedGameStart { get; private set; }

        public string Id { get; }

        public string Username { get; }

        public event Action<IPlayer, int>? OnMovePlayed;
        public event Action<IPlayer, IPlayer>? OnMatch;

        public void ConfirmGameStart()
        {
            Debug.Assert(!HasConfirmedGameStart);
            HasConfirmedGameStart = true;
        }
        public void MakeMove(int column)
        {
            _gameManager.MakeMove(this, column);
        }

        public void OnDraw()
        {
            throw new NotImplementedException();
        }
        public void OnErrorWhileMakingMove(string message)
        {
            throw new NotImplementedException();
        }
        public void OnLost(Connect4Line connect4Line)
        {
            throw new NotImplementedException();
        }
        public void OnLostConnection()
        {
            throw new NotImplementedException();
        }
        public void OnGameCreated(IPlayer player1, IPlayer player2)
        {
            throw new NotImplementedException();
        }
        public void OnOpponentConfirmedGameStart()
        {
            throw new NotImplementedException();
        }
        public void OnOpponentLostconnection()
        {
            throw new NotImplementedException();
        }
        public void OnOpponentMadeMove(int column)
        {
            throw new NotImplementedException();
        }
        public void OnOpponentQuit()
        {
            throw new NotImplementedException();
        }
        public void OpponentReconnected()
        {
            throw new NotImplementedException();
        }
        public void OnReconnected()
        {
            throw new NotImplementedException();
        }
        public void OnWon(Connect4Line connect4Line)
        {
            throw new NotImplementedException();
        }

        private readonly GameManager _gameManager;
        private readonly IHubContext<PlayerHub> _playerHubContext;
    }
}

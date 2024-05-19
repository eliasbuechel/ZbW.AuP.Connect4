using backend.communication.DOTs;
using backend.Data;
using backend.services;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace backend.game
{
    internal abstract class Player : IPlayer
    {
        public Player(string playerId, string username, GameManager gameManager)
        {
            Id = playerId;
            Username = username;
            _gameManager = gameManager;
        }

        public bool HasConfirmedGameStart { get; set; }
        public string Id { get; }
        public string Username { get; }
        public IEnumerable<string> Connections => _connections;

        public event Action<IPlayer, int>? OnMovePlayed;
        public event Action<IPlayer, IPlayer>? OnMatch;

        public IEnumerable<IPlayer> GetOnlinePlayers()
        {
            return _gameManager.GetOnlinePlayersExcept(Id);
        }
        public void Connect(string connection)
        {
            _connections.Add(connection);
            _gameManager.ConnectPlayer(this);
        }
        public void Disconnected(string onnection)
        {
            _connections.Remove(onnection);
            _gameManager.DisconnectPlayer(this);
        }
        public void RequestMatch(IPlayer player)
        {
            if (!_gameManager.RequestMatch(this, player))
                return;

            string playerId = player.Id;
            foreach (var connection in _connections)
                YouRequestedMatch(connection, playerId);
        }
        public void ConfirmGameStart()
        {
            if (HasConfirmedGameStart)
            {
                Debug.Assert(false);
                return;
            }

            HasConfirmedGameStart = true;
            _gameManager.ConfirmedGameStart(this);
        }
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
        public async Task RejectMatch(IPlayer player)
        {
            if (!_gameManager.RejectMatch(this, player))
                return;

            string playerId = player.Id;
            foreach (var connection in _connections)
                await YouRejectedMatch(connection, playerId);
        }
        public IEnumerable<Match> GetGamePlan()
        {
            return _gameManager.GetGamePlan();
        }
        public void PlayMove(int column)
        {
            _gameManager.PlayMove(this, column);
        }
        public Connect4Game GetCurrentGameState()
        {
            return _gameManager.GetCurrentGameState();
        }
        public void QuitGame()
        {
            _gameManager.QuitGame(this);
        }
        public async Task GetGame(string connection)
        {
            if (!_gameManager.IsInGame(this))
                return;

            Connect4GameDTO game = new Connect4GameDTO(_gameManager.GetCurrentGameState());
            await SendGame(connection, game);
        }
        public async Task GetUserDataAsync(string connection)
        {
            PlayerIdentityDTO userData = new PlayerIdentityDTO(this);
            await SendUserData(connection, userData);
        }

        public async Task GetOnlinePlayers(string connection)
        {
            IEnumerable<OnlinePlayerDTO> onlinePlayers = _gameManager.GetOnlinePlayersExcept(Id).Select(p => new OnlinePlayerDTO(p, this)).ToArray();
            await SendOnlinePlayers(connection, onlinePlayers);
        }
        public async Task GetGamePlan(string connection)
        {
            IEnumerable<MatchDTO> gamePlan = _gameManager.GetGamePlan().Select(m => new MatchDTO(m)).ToArray();
            await SendGamePlan(connection, gamePlan);
        }
        public async Task GetCurrentGame(string connection)
        {
            Connect4GameDTO game = new Connect4GameDTO(GetCurrentGameState());
            await SendGame(connection, game);
        }

        public async void PlayerConnected(IPlayer player)
        {
            OnlinePlayerDTO onlinePlayer = new OnlinePlayerDTO(player, this);
            foreach (string connection in Connections)
                await PlayerConnected(connection, onlinePlayer);
        }
        public async void PlayerDisconnected(IPlayer player)
        {
            string playerId = player.Id;
            foreach (string connection in Connections)
                await PlayerDisconnected(connection, playerId);
        }
        public virtual async void RequestedMatch(IPlayer player)
        {
            string playerId = player.Id;
            foreach (string connection in Connections)
                await PlayerRequestedMatch(connection, playerId);
        }
        public async void RejectedMatch(IPlayer player)
        {
            string playerId = player.Id;
            foreach (string connection in Connections)
                await PlayerRejectedMatch(connection, playerId);
        }
        public async void Matched(Match match)
        {
            MatchDTO matchDTO = new MatchDTO(match);
            foreach (string connection in Connections)
                await Matched(connection, matchDTO);
        }
        public async void MatchingEnded(Match match)
        {
            string matchId = match.Id.ToString();
            foreach (string connection in Connections)
                await MatchingEnded(connection, matchId);
        }
        public virtual async void GameStarted(Connect4Game connect4Game)
        {
            Connect4GameDTO connect4GameDTO = new Connect4GameDTO(connect4Game);
            foreach (string connection in Connections)
                await GameStarted(connection, connect4GameDTO);
        }
        public async void GameEnded(GameResult gameResult)
        {
            GameResultDTO gameResultDTO = new GameResultDTO(gameResult);
            foreach (string connection in Connections)
                await GameEnded(connection, gameResultDTO);
        }
        public virtual async void MovePlayed(IPlayer player, Field field)
        {
            string playerId = player.Id;
            FieldDTO fieldDTO = new FieldDTO(field);
            foreach (string connection in Connections)
                await MovePlayed(connection, playerId, fieldDTO);
        }
        public void OpponentConfirmedGameStart()
        {
            foreach (string connection in Connections)
                OpponentConfirmedGameStart(connection);

        }
        public virtual void GameStartConfirmed()
        {
            foreach (string connection in Connections)
                GameStartConfirmed(connection);
        }
        public async void YouConfirmedGameStart()
        {
            foreach (var connection in Connections)
                await YouConfirmedGameStart(connection);
        }

        protected virtual Task PlayerConnected(string connection, OnlinePlayerDTO onlinePlayer)
        {
            return Task.CompletedTask;
        }
        protected virtual Task PlayerDisconnected(string connection, string playerId)
        {
            return Task.CompletedTask;
        }
        protected virtual Task PlayerRequestedMatch(string connection, string playerId)
        {
            return Task.CompletedTask;
        }
        protected virtual Task PlayerRejectedMatch(string connection, string playerId)
        {
            return Task.CompletedTask;
        }
        protected virtual Task Matched(string connection, MatchDTO match)
        {
            return Task.CompletedTask;
        }
        protected virtual Task MatchingEnded(string connection, string matchId)
        {
            return Task.CompletedTask;
        }
        protected virtual Task GameStarted(string connection, Connect4GameDTO connect4Game)
        {
            return Task.CompletedTask;
        }
        protected virtual Task GameEnded(string connection, GameResultDTO gameResult)
        {
            return Task.CompletedTask;
        }
        protected virtual Task MovePlayed(string connection, string playerId, FieldDTO field)
        {
            return Task.CompletedTask;
        }
        protected virtual Task SendUserData(string connection, PlayerIdentityDTO userData)
        {
            return Task.CompletedTask;
        }
        protected virtual Task SendOnlinePlayers(string connection, IEnumerable<OnlinePlayerDTO> onlinePlayers)
        {
            return Task.CompletedTask;
        }
        protected virtual Task SendGamePlan(string connection, IEnumerable<MatchDTO> gamePlan)
        {
            return Task.CompletedTask;
        }
        protected virtual Task SendGame(string connection, Connect4GameDTO game)
        {
            return Task.CompletedTask;
        }
        protected virtual Task YouRequestedMatch(string connection, string playerId)
        {
            return Task.CompletedTask;
        }
        protected virtual Task YouRejectedMatch(string connection, string playerId)
        {
            return Task.CompletedTask;
        }
        protected virtual Task OpponentConfirmedGameStart(string connection)
        {
            return Task.CompletedTask;
        }
        protected virtual Task GameStartConfirmed(string connection)
        {
            return Task.CompletedTask;
        }
        protected virtual Task YouConfirmedGameStart(string connection)
        {
            return Task.CompletedTask;
        }


        protected readonly GameManager _gameManager;
        private readonly ICollection<string> _connections = new List<string>();
    }
}

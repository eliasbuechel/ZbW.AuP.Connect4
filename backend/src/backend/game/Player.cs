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
            Debug.Assert(!HasConfirmedGameStart);
            HasConfirmedGameStart = true;
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
        public async void RequestedMatch(IPlayer player)
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
        public async void GameStarted(Connect4Game connect4Game)
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
        public async void MovePlayed(IPlayer player, Field field)
        {
            string playerId = player.Id;
            FieldDTO fieldDTO = new FieldDTO(field);
            foreach (string connection in Connections)
                await MovePlayed(connection, playerId, fieldDTO);
        }

        protected abstract Task PlayerConnected(string connection, OnlinePlayerDTO onlinePlayer);
        protected abstract Task PlayerDisconnected(string connection, string playerId);
        protected abstract Task PlayerRequestedMatch(string connection, string playerId);
        protected abstract Task PlayerRejectedMatch(string connection, string playerId);
        protected abstract Task Matched(string connection, MatchDTO match);
        protected abstract Task MatchingEnded(string connection, string matchId);
        protected abstract Task GameStarted(string connection, Connect4GameDTO connect4Game);
        protected abstract Task GameEnded(string connection, GameResultDTO gameResult);
        protected abstract Task MovePlayed(string connection, string playerId, FieldDTO field);
        protected abstract Task SendUserData(string connection, PlayerIdentityDTO userData);
        protected abstract Task SendOnlinePlayers(string connection, IEnumerable<OnlinePlayerDTO> onlinePlayers);
        protected abstract Task SendGamePlan(string connection, IEnumerable<MatchDTO> gamePlan);
        protected abstract Task SendGame(string connection, Connect4GameDTO game);
        protected abstract Task YouRequestedMatch(string connection, string playerId);
        protected abstract Task YouRejectedMatch(string connection, string playerId);

        private readonly GameManager _gameManager;
        private readonly ICollection<string> _connections = new List<string>();
    }
}

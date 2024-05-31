using backend.communication.DOTs;
using backend.Data;
using backend.game.entities;
using backend.services;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Numerics;

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

        public string Id { get; }
        public string Username { get; }
        public bool HasConfirmedGameStart { get; set; }
        public int HintsLeft => _hintsLeft;
        public int? CurrentHint => _currentHint;
        public ICollection<string> Connections => _connections;

        public event Action<IPlayer, int>? OnMovePlayed;
        public event Action<IPlayer, IPlayer>? OnMatch;


        // REQUESTS
        public async Task RequestMatch(IPlayer player)
        {
            if (!_gameManager.RequestMatch(this, player))
                return;

            string playerId = player.Id;
            foreach (var connection in _connections)
                await YouRequestedMatch(connection, playerId);
        }
        public Task ConfirmGameStartAsync()
        {
            if (HasConfirmedGameStart)
            {
                Debug.Assert(false);
                return Task.CompletedTask;
            }

            HasConfirmedGameStart = true;
            _gameManager.ConfirmedGameStart(this);
            return Task.CompletedTask;
        }
        public bool HasRequestedMatch(IPlayer you)
        {
            return _gameManager.HasRequestedMatch(this, you);
        }
        public bool HasMatched(IPlayer player)
        {
            return _gameManager.HasMatched(this, player);
        }
        public async Task AcceptMatchAsync(IPlayer player)
        {
            _gameManager.AcceptMatch(this, player);
            await Task.CompletedTask;
        }
        public async Task RejectMatchAsync(IPlayer player)
        {
            if (!_gameManager.RejectMatch(this, player))
                return;

            string playerId = player.Id;
            foreach (var connection in _connections)
                await YouRejectedMatch(connection, playerId);
        }
        public Task PlayMoveAsync(int column)
        {
            _gameManager.PlayMove(this, column);
            return Task.CompletedTask;
        }
        public Connect4Game GetCurrentGameState()
        {
            return _gameManager.GetCurrentGameState();
        }
        public Task QuitGameAsync()
        {
            _gameManager.QuitGame(this);
            return Task.CompletedTask;
        }
        public async Task GetGameAsync(string connection)
        {
            if (!_gameManager.IsInGame(this))
                return;

            Connect4GameDTO game = new Connect4GameDTO(_gameManager.GetCurrentGameState());
            await SendGame(connection, game);
        }
        public async Task GetUserDataAsync(string connection)
        {
            PlayerInfoDTO userData = new PlayerInfoDTO(this);
            await SendUserData(connection, userData);
        }

        public async Task GetOnlinePlayersAsync(string connection)
        {
            ConnectedPlayersDTO connectedPlayers = _gameManager.GetOnlinePlayersExcept(this);
            await SendOnlinePlayers(connection, connectedPlayers);
        }
        public async Task GetGamePlanAsync(string connection)
        {
            IEnumerable<MatchDTO> gamePlan = _gameManager.GetGamePlan().Select(m => new MatchDTO(m)).ToArray();
            await SendGamePlan(connection, gamePlan);
        }
        public async Task GetCurrentGameAsync(string connection)
        {
            Connect4GameDTO game = new Connect4GameDTO(GetCurrentGameState());
            await SendGame(connection, game);
        }
        public async Task GetBestlist(string connection)
        {
            IEnumerable<GameResult> bestlist = _gameManager.GetBestlist();
            await SendBestList(connection, bestlist);
        }


        // RESPONSES
        public async void PlayerConnected(IPlayer player)
        {
            ConnectedPlayerDTO onlinePlayer = new ConnectedPlayerDTO(player, this);
            foreach (string connection in Connections)
                await PlayerConnected(connection, onlinePlayer);
        }
        public async void PlayerDisconnected(IPlayer player)
        {
            string playerId = player.Id;
            foreach (string connection in Connections)
                await PlayerDisconnected(connection, playerId);
        }
        internal async void OpponentRoboterPlayerConnected(IPlayer opponentRoboterPlayer)
        {
            ConnectedPlayerDTO opponentRoboterPlayerDTO = new ConnectedPlayerDTO(opponentRoboterPlayer, this);
            foreach (string connection in Connections)
                await OpponentRoboterPlayerConnected(connection, opponentRoboterPlayerDTO);
        }
        internal async void OpponentRoboterPlayerDisconnected(IPlayer opponentRoboterPlayer)
        {
            ConnectedPlayerDTO opponentRoboterPlayerDTO = new ConnectedPlayerDTO(opponentRoboterPlayer, this);
            foreach (string connection in Connections)
                await OpponentRoboterPlayerDisconnected(connection, opponentRoboterPlayerDTO);
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
            _hintsLeft = MAX_HINTS;

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
            _currentHint = null;

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
        public async void GetHint()
        {
            if (_hintsLeft <= 0)
                return;

            _hintsLeft--;
            int hint = _gameManager.GetBestMove(this);
            _currentHint = hint;
            
            foreach (var connection in Connections)
                await SendHint(connection, hint);
        }
        public async void SendBestList(IEnumerable<GameResult> bestlist)
        {
            foreach (var connection in Connections)
                await SendBestList(connection, bestlist);
        }

        protected virtual Task PlayerConnected(string connection, ConnectedPlayerDTO onlinePlayer)
        {
            return Task.CompletedTask;
        }
        protected virtual Task PlayerDisconnected(string connection, string playerId)
        {
            return Task.CompletedTask;
        }
        protected virtual Task OpponentRoboterPlayerConnected(string connection, ConnectedPlayerDTO opponentRoboterPlayer)
        {
            return Task.CompletedTask;
        }
        protected virtual Task OpponentRoboterPlayerDisconnected(string connection, ConnectedPlayerDTO opponentRoboterPlayer)
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
        protected virtual Task SendUserData(string connection, PlayerInfoDTO userData)
        {
            return Task.CompletedTask;
        }
        protected virtual Task SendOnlinePlayers(string connection, ConnectedPlayersDTO connectedPlayers)
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
        protected virtual Task SendBestList(string connection, IEnumerable<GameResult> bestlist)
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
        protected virtual Task SendHint(string connection, int hint)
        {
            return Task.CompletedTask;
        }

        protected readonly GameManager _gameManager;
        private readonly ICollection<string> _connections = new List<string>();
        private int _hintsLeft = MAX_HINTS;
        private int? _currentHint = null;
        private const int MAX_HINTS = 3;
    }
}

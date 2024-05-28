using backend.game;
using backend.game.entities;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace backend.services
{
    internal class GameManager
    {
        public GameManager(PlayerConnectionManager playerConnectionManager, Func<Match, Connect4Game> getConnect4Game, GameResultsService gameResultsService)
        {
            _playerConnectionManager = playerConnectionManager;
            _getConnect4Game = getConnect4Game;
            _gameResultsService = gameResultsService;
        }

        public event Action<IPlayer, IPlayer>? OnGameStarted;

        public IEnumerable<Match> GetGamePlan()
        {
            return _gamePlan.ToArray();
        }
        public IEnumerable<IPlayer> GetOnlinePlayersExcept(string id)
        {
            return _playerConnectionManager.OnlinePlayers.Where(p => p.Id != id);
        }
        internal IEnumerable<GameResult> GetBestlist()
        {
            return _gameResultsService.Bestlist;
        }
        public void ConnectPlayer(IPlayer player)
        {
            _playerConnectionManager.ConnectPlayer(player);
        }
        public void DisconnectPlayer(IPlayer player)
        {
            _playerConnectionManager.DisconnectPlayer(player, PlayerQuit);
        }
        public bool HasRequestedMatch(IPlayer requester, IPlayer opponent)
        {
            foreach (MatchRequest matchRequest in _matchRequests)
                if (matchRequest.Requester == requester && matchRequest.Opponent == opponent)
                    return true;

            return false;
        }
        public bool RequestMatch(IPlayer requester, IPlayer opponent)
        {
            foreach (var request in _matchRequests)
            {
                if (request.Requester == requester)
                {
                    requester.RejectedMatch(opponent);
                    return false;
                }
            }
            _matchRequests.Add(new MatchRequest(requester, opponent));

            opponent.RequestedMatch(requester);
            return true;
        }
        public bool HasMatched(IPlayer player1, IPlayer player2)
        {
            foreach (Match match in _gamePlan)
            {
                if (match.Player1 == player1 && match.Player2 == player2)
                    return true;

                if (match.Player1 == player2 && match.Player2 == player1)
                    return true;
            }

            return false;
        }
        public void AcceptMatch(IPlayer player, IPlayer requester)
        {
            foreach (MatchRequest matchRequest in _matchRequests)
            {
                if (matchRequest.Requester == requester && matchRequest.Opponent == player)
                {
                    _matchRequests = new ConcurrentBag<MatchRequest>(_matchRequests.Where(x => x != matchRequest));
                    Match match = new Match(matchRequest);

                    foreach (IPlayer p in _playerConnectionManager.OnlinePlayers)
                        p.Matched(match);

                    _gamePlan.Enqueue(match);

                    TryStartGame();
                    return;
                }
            }
        }
        public bool RejectMatch(Player player, IPlayer requester)
        {
            foreach (MatchRequest matchRequest in _matchRequests)
            {
                if (matchRequest.Requester == requester && matchRequest.Opponent == player)
                {
                    _matchRequests = new ConcurrentBag<MatchRequest>(_matchRequests.Where(x => x != matchRequest));
                    requester.RejectedMatch(player);
                    return true;
                }

            }

            Debug.Assert(false);
            return false;
        }
        public void PlayMove(Player player, int column)
        {
            if (_activeGame == null)
            {
                return;
            }

            _activeGame.PlayMove(player, column);
        }
        public void QuitGame(Player player)
        {
            Debug.Assert(_activeGame != null);
            _activeGame.PlayerQuit(player);
        }
        public bool IsInGame(IPlayer player)
        {
            if (_activeGame == null)
                return false;

            if (_activeGame.Match.Player1 == player || _activeGame.Match.Player2 == player)
                return true;

            return false;
        }
        public Connect4Game GetCurrentGameState()
        {
            Debug.Assert(_activeGame != null);
            return _activeGame;
        }

        private void StartNewGame(Match match)
        {
            Debug.Assert(_activeGame == null);
            _activeGame = _getConnect4Game(match);
            _activeGame.OnGameEnded += OnGameEnded;

            _activeGame.Initialize();
        }
        private async void OnGameEnded(GameResult gameResult)
        {
            if (_activeGame == null)
            {
                Debug.Assert(false);
                return;
            }

            await _gameResultsService.Add(gameResult);
            IEnumerable<GameResult> bestlist = _gameResultsService.Bestlist;
            foreach (var player in _playerConnectionManager.OnlinePlayers)
                player.SendBestList(bestlist);

            _activeGame.OnGameEnded -= OnGameEnded;
            _activeGame.Dispose();
            _activeGame = null;

            Match? match;
            if (!_gamePlan.TryDequeue(out match))
            {
                Debug.Assert(false);
                return;
            }

            foreach (var onlinePlayer in _playerConnectionManager.OnlinePlayers)
                onlinePlayer.MatchingEnded(match);

            TryStartGame();
        }
        private void PlayerQuit(IPlayer player)
        {
            while (true)
            {
                MatchRequest? foundMatchRequest = null;

                foreach (MatchRequest matchRequest in _matchRequests)
                {
                    if (matchRequest.Requester == player || matchRequest.Opponent == player)
                    {
                        foundMatchRequest = matchRequest;
                        break;
                    }
                }

                if (foundMatchRequest == null)
                    break;

                MatchRequest value;
                _matchRequests = new ConcurrentBag<MatchRequest>(_matchRequests.Where(x => x != foundMatchRequest).ToArray());
            }

            while (true)
            {
                Match? foundMatch = null;

                foreach (Match match in _gamePlan)
                {
                    if (match.Player1 == player || match.Player2 == player)
                    {
                        foundMatch = match;
                        break;
                    }
                }

                if (foundMatch == null)
                    break;

                List<Match> gamePlan = new List<Match>(_gamePlan);
                gamePlan.Remove(foundMatch);
                _gamePlan = new ConcurrentQueue<Match>(gamePlan);
            }
        }
        private void TryStartGame()
        {
            if (_activeGame != null)
                return;

            Match? match;
            if (!_gamePlan.TryPeek(out match))
                return;

            StartNewGame(match);
        }

        public void ConfirmedGameStart(IPlayer player)
        {
            Debug.Assert(_activeGame != null);
            _activeGame.ConnfirmedGameStart(player);
        }
        public int GetBestMove(IPlayer player)
        {
            if (_activeGame == null)
            {
                return -1;
            }

            return _activeGame.GetBestMove(player);
        }

        private Connect4Game? _activeGame = null;
        private readonly GameResultsService _gameResultsService;
        private readonly Func<Match, Connect4Game> _getConnect4Game;
        private readonly PlayerConnectionManager _playerConnectionManager;
        private ConcurrentQueue<Match> _gamePlan = new ConcurrentQueue<Match>();
        private ConcurrentBag<MatchRequest> _matchRequests = new ConcurrentBag<MatchRequest>();
    }
}
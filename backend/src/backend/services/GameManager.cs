using backend.communication.DOTs;
using backend.communication.signalR;
using backend.game;
using backend.game.entities;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace backend.services
{
    internal class GameManager : IDisposable
    {
        public GameManager(ConnectedPlayerProvider connectedPlayerProvider, Func<Match, Game> getConnect4Game, GameResultsService gameResultsService)
        {
            _connectedPlayerProvider = connectedPlayerProvider;
            _getConnect4Game = getConnect4Game;
            _gameResultsService = gameResultsService;

            _connectedPlayerProvider.OnPlayerDisconnected += PlayerQuit;
        }

        public event Action<IPlayer, IPlayer>? OnGameStarted;

        public IEnumerable<Match> GetGamePlan()
        {
            return _gamePlan.ToArray();
        }
        public ConnectedPlayersDTO GetOnlinePlayersExcept(IPlayer player)
        {
            ConnectedPlayersDTO connectedPlayersDTO = new ConnectedPlayersDTO(_connectedPlayerProvider, player);
            return connectedPlayersDTO;
        }
        internal IEnumerable<GameResult> GetBestlist()
        {
            return _gameResultsService.Bestlist;
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

                    foreach (ToPlayerHub<WebPlayerHub> p in _connectedPlayerProvider.WebPlayers)
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
                return;

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
        public Game GetCurrentGameState()
        {
            Debug.Assert(_activeGame != null);
            return _activeGame;
        }
        public void Dispose()
        {
            if (_disposed)
            {
                Debug.Assert(false);
                return;
            }

            _disposed = true;

            _connectedPlayerProvider.OnPlayerDisconnected -= PlayerQuit;
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
            foreach (var player in _connectedPlayerProvider.WebPlayers)
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

            foreach (var onlinePlayer in _connectedPlayerProvider.WebPlayers)
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

        public void WatchGame(IPlayer player)
        {
            if (_activeGame == null)
            {
                Debug.Assert(false);
                return;
            }

            _activeGame.AddWatcher(player);
        }

        public void StopWatchingGame(IPlayer player)
        {
            if (_activeGame == null)
            {
                Debug.Assert(false);
                return;
            }

            _activeGame.RemoveWatcher(player);
        }


        private bool _disposed;
        private Game? _activeGame = null;
        private readonly GameResultsService _gameResultsService;
        private readonly Func<Match, Game> _getConnect4Game;
        private readonly ConnectedPlayerProvider _connectedPlayerProvider;
        private ConcurrentQueue<Match> _gamePlan = new ConcurrentQueue<Match>();
        private ConcurrentBag<MatchRequest> _matchRequests = new ConcurrentBag<MatchRequest>();
    }
}
using backend.game;
using System.Diagnostics;

namespace backend.services
{
    internal class GameManager
    {
        public GameManager(PlayerConnectionManager playerConnectionManager, Func<Match, Connect4Game> getConnect4Game)
        {
            _playerConnectionManager = playerConnectionManager;
            _getConnect4Game = getConnect4Game;
        }

        public event Action<IPlayer, IPlayer>? OnGameStarted;

        public IEnumerable<Match> GetGamePlan()
        {
            lock (_gamePlanLock)
            {
                return _gamePlan.ToArray();
            }
        }
        public IEnumerable<IPlayer> GetOnlinePlayersExcept(string id)
        {
            return _playerConnectionManager.OnlinePlayers.Where(p => p.Id != id);
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
            lock (_matchRequestsLock)
            {
                foreach (MatchRequest matchRequest in _matchRequests)
                    if (matchRequest.Requester == requester && matchRequest.Opponent == opponent)
                        return true;
            }
            return false;
        }
        public void RequestMatch(IPlayer requester, IPlayer opponent)
        {
            lock (_matchRequestsLock)
            {
                foreach (var request in _matchRequests)
                {
                    if (request.Requester == requester)
                    {
                        requester.RejectedMatch(opponent);
                        return;
                    }
                }
                _matchRequests.Add(new MatchRequest(requester, opponent));
            }
            opponent.RequestedMatch(requester);
        }
        public bool HasMatched(Player player1, IPlayer player2)
        {
            lock (_gamePlanLock)
            {
                foreach (Match match in _gamePlan)
                {
                    if (match.Player1 == player1 && match.Player2 == player2)
                        return true;

                    if (match.Player1 == player2 && match.Player2 == player1)
                        return true;
                }
            }
            return false;
        }
        public void AcceptMatch(IPlayer player, IPlayer requester)
        {
            lock (_matchRequestsLock)
            {
                foreach (MatchRequest matchRequest in _matchRequests)
                {
                    if (matchRequest.Requester == requester && matchRequest.Opponent == player)
                    {
                        _matchRequests.Remove(matchRequest);
                        Match match = new Match(matchRequest);

                        foreach (IPlayer p in _playerConnectionManager.OnlinePlayers)
                            p.Matched(match);

                        _gamePlan.Enqueue(match);

                        TryStartGame();
                        return;
                    }
                }
            }
        }
        public void RejectMatch(Player player, IPlayer requester)
        {
            lock (_matchRequestsLock)
            {
                foreach (MatchRequest matchRequest in _matchRequests)
                {
                    if (matchRequest.Requester == requester && matchRequest.Opponent == player)
                    {
                        _matchRequests.Remove(matchRequest);
                        requester.RejectedMatch(player);
                        return;
                    }

                }
            }
            Debug.Assert(false);
        }
        public void PlayMove(Player player, int column)
        {
            if (_activeGame == null)
            {
                Debug.Assert(false);
                return;
            }

            _activeGame.PlayMove(player, column);
        }
        public void QuitGame(Player player)
        {
            Debug.Assert(_activeGame != null);
            _activeGame.PlayerQuit(player);
        }
        public bool HasGameStarted(IPlayer player)
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
        private void OnGameEnded()
        {
            if (_activeGame == null)
            {
                Debug.Assert(false);
                return;
            }

            _activeGame.OnGameEnded -= OnGameEnded;
            _activeGame.Dispose();
            _activeGame = null;
            _gamePlan.Dequeue();

            TryStartGame();
        }
        private void PlayerQuit(IPlayer player)
        {
            while (true)
            {
                MatchRequest? foundMatchRequest = null;

                lock (_matchRequestsLock)
                {
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

                    _matchRequests.Remove(foundMatchRequest);
                }
            }

            while (true)
            {
                Match? foundMatch = null;

                lock (_gamePlanLock)
                {
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
                    _gamePlan = new Queue<Match>(gamePlan);
                }
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

        private object _gamePlanLock = new object();
        private object _matchRequestsLock = new object();
        private Connect4Game? _activeGame = null;
        private readonly PlayerConnectionManager _playerConnectionManager;
        private readonly Func<Match, Connect4Game> _getConnect4Game;
        private Queue<Match> _gamePlan = new Queue<Match>();
        private readonly List<MatchRequest> _matchRequests = new List<MatchRequest>();
    }
}

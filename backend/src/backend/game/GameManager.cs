using backend.services;
using System.Diagnostics;

namespace backend.game
{
    internal class GameManager
    {
        public GameManager(PlayerManager playerManager)
        {
            _playerManager = playerManager;
        }
        public event Action<IPlayer, IPlayer>? OnGameStarted;

        //public void AddPlayer(IPlayer player)
        //{
        //    Debug.Assert(!_activePlayers.Contains(player));
        //    _activePlayers.Add(player);
        //    player.OnMovePlayed += OnPlayerPlayedMove;
        //    player.OnMatch += OnMatch;
        //    OnGameStarted += player.OnGameCreated;
        //}
        //public void RemovePlayer(IPlayer player)
        //{
        //    Debug.Assert(_activePlayers.Contains(player));
        //    player.OnMovePlayed -= OnPlayerPlayedMove;
        //    player.OnMatch -= OnMatch;
        //    _activePlayers.Remove(player);
        //}
        //public void MakeMove(IPlayer player, int column)
        //{
        //    Debug.Assert(_activeGame != null);

        //    try
        //    {
        //        _activeGame.PlayMove(player, column);
        //    }
        //    catch (Exception ex)
        //    {
        //        player.OnErrorWhileMakingMove(ex.Message);
        //    }
        //}

        //private void OnPlayerPlayedMove(IPlayer player, int column)
        //{
        //    if (_activeGame == null)
        //    {
        //        player.OnErrorWhileMakingMove("No game is active");
        //        return;
        //    }

        //    try
        //    {
        //        _activeGame.PlayMove(player, column);
        //    }
        //    catch (Exception ex)
        //    {
        //        player.OnErrorWhileMakingMove(ex.Message);
        //        return;
        //    }
        //}
        //private void OnMatch(IPlayer player1, IPlayer player2)
        //{
        //    _gamePlan.Enqueue(new Tuple<IPlayer, IPlayer>(player1, player2));
        //    if (_activeGame == null)
        //        StartNextGame();
        //}
        //private void StartNextGame()
        //{
        //    var players = _gamePlan.Dequeue();
        //    _activeGame = new Game(players.Item1, players.Item2);
        //    OnGameStarted?.Invoke(_activeGame.Player1, _activeGame.Player2);
        //}

        public void RequestMatch(IPlayer requester, IPlayer opponent)
        {
            lock(_matchRequestsLock)
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
        public bool HasRequestedMatch(IPlayer requester, IPlayer opponent)
        {
            lock(_matchRequestsLock)
            {
                foreach (MatchRequest matchRequest in _matchRequests)
                    if (matchRequest.Requester == requester && matchRequest.Opponent == opponent)
                        return true;
            }
            return false;
        }
        public bool HasMatched(Player player1, IPlayer player2)
        {
            lock(_gamePlanLock)
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
                        lock (_gamePlanLock)
                        {
                            _gamePlan.Enqueue(match);
                        }

                        foreach (IPlayer p in _playerManager.OnlinePlayers)
                            p.Matched(match);

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
        private void PlayerQuit(IPlayer player)
        {
            while (true)
            {
                MatchRequest? foundMatchRequest = null;

                lock(_matchRequestsLock)
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

        public IEnumerable<Match> GetGamePlan()
        {
            lock (_gamePlanLock)
            {
                return _gamePlan.ToArray();
            }
        }

        public void ConnectPlayer(IPlayer player)
        {
            _playerManager.ConnectPlayer(player);
        }

        internal void DisconnectPlayer(IPlayer player)
        {
            _playerManager.DisconnectPlayer(player, PlayerQuit);
        }

        public IEnumerable<IPlayer> GetOnlinePlayersExcept(string id)
        {
            return _playerManager.OnlinePlayers.Where(p => p.Id != id);
        }

        private object _gamePlanLock = new object();
        private object _matchRequestsLock = new object();
        //private Game? _activeGame = null;
        private Queue<Match> _gamePlan = new Queue<Match>(); 
        private readonly List<MatchRequest> _matchRequests = new List<MatchRequest>();
        private readonly PlayerManager _playerManager;
    }
}

﻿using backend.game;
using System.Diagnostics;

namespace backend.services
{
    internal class GameManager
    {
        private static int instanceCount = 0;
        public GameManager(PlayerConnectionManager playerConnectionManager)
        {
            instanceCount++;
            Debug.Assert(instanceCount < 2); // only one instance allowed

            _playerConnectionManager = playerConnectionManager;
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
            Debug.Assert(_activeGame != null);
            if (_activeGame.PlayMove(player, column))
                return;

            StartNextGame();
        }
        internal void QuitGame(Player player)
        {
            Debug.Assert(_activeGame != null);
            _activeGame.Quit(player);
            StartNextGame();
        }
        internal bool HasGameStarted(IPlayer player)
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
            {
                _activeGame = null;
                return;
            }

            _activeGame = new Connect4Game(match);
        }
        private void StartNextGame()
        {
            Debug.Assert(_activeGame != null);

            foreach (Player player in _playerConnectionManager.OnlinePlayers)
                if (player != _activeGame.Match.Player1 && player != _activeGame.Match.Player2)
                    player.GameEnded(new GameResult(null, null));

            _activeGame = null;
            _gamePlan.Dequeue();
            TryStartGame();
        }


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


        private object _gamePlanLock = new object();
        private object _matchRequestsLock = new object();
        private Connect4Game? _activeGame = null;
        private readonly PlayerConnectionManager _playerConnectionManager;
        private Queue<Match> _gamePlan = new Queue<Match>();
        private readonly List<MatchRequest> _matchRequests = new List<MatchRequest>();
    }
}
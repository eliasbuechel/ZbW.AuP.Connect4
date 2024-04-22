using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace backend.game
{
    internal class GameManager
    {
        public event Action<IPlayer, IPlayer>? OnGameStarted;

        public IEnumerable<IPlayer> ActivePlayers => _activePlayers;

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
        private void OnMatch(IPlayer player1, IPlayer player2)
        {
            _gamePlan.Enqueue(new Tuple<IPlayer, IPlayer>(player1, player2));
            if (_activeGame == null)
                StartNextGame();
        }

        private void StartNextGame()
        {
            var players = _gamePlan.Dequeue();
            _activeGame = new Game(players.Item1, players.Item2);
            OnGameStarted?.Invoke(_activeGame.Player1, _activeGame.Player2);
        }

        internal void RequestMatch(IPlayer requester, IPlayer opponent)
        {
            foreach (var request in _requestedGamePlan)
            {
                if (request.Item1 == requester)
                {
                    requester.RejectedMatch(opponent);
                    return;
                }
            }

            _requestedGamePlan.Add(new Tuple<IPlayer, IPlayer>(requester, opponent));
            opponent.RequestedMatch(requester);
        }

        public bool HasRequestedMatch(IPlayer requester, IPlayer opponent)
        {
            foreach (var request in _requestedGamePlan)
                if (request.Item1 == requester && request.Item2 == opponent)
                    return true;

            return false;
        }

        public bool HasMatched(Player player1, IPlayer player2)
        {
            foreach (var match in _gamePlan)
            {
                if (match.Item1 == player1 && match.Item2 == player2)
                    return true;

                if (match.Item1 == player2 && match.Item2 == player1)
                    return true;
            }

            return false;
        }

        public void AcceptMatch(IPlayer player, IPlayer requester)
        {
            foreach (var request in _requestedGamePlan)
            {
                if (request.Item1 == requester && request.Item2 == player)
                {
                    _requestedGamePlan.Remove(request);
                    _gamePlan.Enqueue(request);

                    player.Matched(requester);
                    requester.Matched(player);

                    return;
                }
            }
        }

        internal void RejectMatch(Player player, IPlayer requester)
        {
            foreach (var request in _requestedGamePlan)
            {
                if (request.Item1 == requester && request.Item2 == player)
                {
                    _requestedGamePlan.Remove(request);
                    requester.RejectedMatch(player);
                    return;
                }

            }
            
            Debug.Assert(false);
        }

        internal void PlayerQuit(IPlayer player)
        {
            while (true)
            {
                Tuple<IPlayer, IPlayer>? foundRequest = null;

                foreach (var request in _requestedGamePlan)
                {
                    if (request.Item1 == player || request.Item2 == player)
                    {
                        foundRequest = request;
                        break;
                    }
                }

                if (foundRequest == null)
                    break;

                _requestedGamePlan.Remove(foundRequest);
            }

            while (true)
            {
                Tuple<IPlayer, IPlayer>? foundMatch = null;

                foreach (var match in _gamePlan)
                {
                    if (match.Item1 == player || match.Item2 == player)
                    {
                        foundMatch = match;
                        break;
                    }
                }

                if (foundMatch == null)
                    break;

                List<Tuple<IPlayer, IPlayer>> gamePlan = new List<Tuple<IPlayer, IPlayer>>(_gamePlan);
                gamePlan.Remove(foundMatch);
                _gamePlan = new Queue<Tuple<IPlayer, IPlayer>>(gamePlan);
            }
        }

        private Game? _activeGame = null;
        private readonly List<Tuple<IPlayer, IPlayer>> _requestedGamePlan = new List<Tuple<IPlayer, IPlayer>>(); // first = requester, second = opponent
        private Queue<Tuple<IPlayer, IPlayer>> _gamePlan = new Queue<Tuple<IPlayer, IPlayer>>(); 
        private readonly List<IPlayer> _activePlayers = new List<IPlayer>();
    }
}

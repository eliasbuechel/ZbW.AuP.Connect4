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
                    requester.DeclineMatch(opponent);
                    return;
            }

            _requestedGamePlan.Add(new Tuple<IPlayer, IPlayer>(requester, opponent));
            opponent.RequestedMatch(opponent);
        }

        private Game? _activeGame = null;
        private readonly List<Tuple<IPlayer, IPlayer>> _requestedGamePlan = new List<Tuple<IPlayer, IPlayer>>(); // first = requester, second = opponent
        private readonly Queue<Tuple<IPlayer, IPlayer>> _gamePlan = new Queue<Tuple<IPlayer, IPlayer>>(); 
        private readonly List<IPlayer> _activePlayers = new List<IPlayer>();
    }
}

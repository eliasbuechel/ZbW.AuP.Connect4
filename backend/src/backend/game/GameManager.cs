using System.Diagnostics;

namespace backend.game
{
    internal class GameManager
    {
        public event Action<IPlayer, IPlayer>? OnGameStarted;

        public IEnumerable<IPlayer> ActivePlayers => _activePlayers;

        public void AddPlayer(IPlayer player)
        {
            Debug.Assert(!_activePlayers.Contains(player));
            _activePlayers.Add(player);
            player.OnMovePlayed += OnPlayerPlayedMove;
            player.OnMatch += OnMatch;
            OnGameStarted += player.OnGameCreated;
        }
        public void RemovePlayer(IPlayer player)
        {
            Debug.Assert(_activePlayers.Contains(player));
            player.OnMovePlayed -= OnPlayerPlayedMove;
            player.OnMatch -= OnMatch;
            _activePlayers.Remove(player);
        }
        public void MakeMove(IPlayer player, int column)
        {
            Debug.Assert(_activeGame != null);

            try
            {
                _activeGame.PlayMove(player, column);
            }
            catch (Exception ex)
            {
                player.OnErrorWhileMakingMove(ex.Message);
            }
        }

        private void OnPlayerPlayedMove(IPlayer player, int column)
        {
            if (_activeGame == null)
            {
                player.OnErrorWhileMakingMove("No game is active");
                return;
            }

            try
            {
                _activeGame.PlayMove(player, column);
            }
            catch (Exception ex)
            {
                player.OnErrorWhileMakingMove(ex.Message);
                return;
            }
        }
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

        private Game? _activeGame = null;
        private readonly Queue<Tuple<IPlayer, IPlayer>> _gamePlan = new Queue<Tuple<IPlayer, IPlayer>>(); 
        private readonly List<IPlayer> _activePlayers = new List<IPlayer>();
    }
}

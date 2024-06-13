using backend.game;
using backend.game.entities;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace backend.services
{
    internal class GameManager
    {
        public GameManager(
            Func<Match, Game> getConnect4Game,
            GameResultsService gameResultsService
            )
        {
            _getConnect4Game = getConnect4Game;
            _gameResultsService = gameResultsService;
        }

        public event Action<Player, Player>? OnRequestedMatch;
        public event Action<Player, Player>? OnRejectedMatch;
        public event Action<Match>? OnMatched;
        public event Action<Game>? OnGameStarted;
        public event Action<Player>? OnConfirmedGameStart;
        public event Action<Player, Field>? OnMovePlayed;
        public event Action<Player, int>? OnSendHint;
        public event Action<GameResult>? OnGameEnded;

        public Game? Game => _activeGame;
        public IEnumerable<Match> GamePlan => _gamePlan.ToArray();

        // requests
        public void GetHint(Player player)
        {
            int column = GetBestMove(player);
            SendHint(player, column);
        }


        public bool RequestMatch(Player requester, Player opponent)
        {
            RequestedMatch(requester, opponent);
            return true;
        }
        public void AcceptMatch(Player accepter, Player opponent)
        {
            accepter.MatchingRequests.Remove(opponent);
            accepter.Matching = opponent;
            opponent.Matching = accepter;

            Match match = new Match(accepter, opponent);
            Matched(match);
            _gamePlan.Enqueue(match);
            TryStartGame();
        }
        public void RejectMatch(Player rejecter, Player opponent)
        {
            rejecter.MatchingRequests.Remove(opponent);
            RejectedMatch(rejecter, opponent);
        }
        public void ConfirmGameStart(Player player)
        {
            if (_activeGame == null)
            {
                Debug.Assert(false);
                return;
            }

            _activeGame.ConnfirmGameStart(player);
            ConfirmedGameStart(player);
        }
        public void PlayMove(Player player, int column)
        {
            if (_activeGame == null)
                return;

            _activeGame.PlayMove(player, column);
        }
        public int GetBestMove(Player player)
        {
            if (_activeGame == null)
            {
                return -1;
            }

            return _activeGame.GetBestMove(player);
        }
        public void QuitGame(Player player)
        {
            Debug.Assert(_activeGame != null);
            _activeGame.PlayerQuit(player);
        }
        public void PlayerDisconnected(Player player)
        {
            if (_activeGame != null)
                QuitGame(player);

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


        private void RequestedMatch(Player requester, Player opponent)
        {
            opponent.MatchingRequests.Add(requester);
            OnRequestedMatch?.Invoke(requester, opponent);
        }
        private void RejectedMatch(Player rejecter, Player opponent)
        {
            opponent.MatchingRequests.Remove(opponent);
            OnRejectedMatch?.Invoke(rejecter, opponent);
        }
        private void Matched(Match match)
        {
            if (match.Player1.MatchingRequests.Contains(match.Player2))
                match.Player1.MatchingRequests.Remove(match.Player2);
            if (match.Player2.MatchingRequests.Contains(match.Player1))
                match.Player2.MatchingRequests.Remove(match.Player1);

            match.Player1.Matching = match.Player2;
            match.Player2.Matching = match.Player1;
            OnMatched?.Invoke(match);
        }
        private void GameStarted(Game game)
        {
            game.Match.Player1.IsInGame = true;
            game.Match.Player2.IsInGame = true;
            OnGameStarted?.Invoke(game);
        }
        private void GameEnded(GameResult gameResult)
        {
            OnGameEnded?.Invoke(gameResult);
        }
        private void ConfirmedGameStart(Player player)
        {
            OnConfirmedGameStart?.Invoke(player);
        }
        private void MovePlayed(Player player, Field field)
        {
            OnMovePlayed?.Invoke(player, field);
        }
        private void SendHint(Player player, int column)
        {
            OnSendHint?.Invoke(player, column);
        }

        private void StartNewGame(Match match)
        {
            Debug.Assert(_activeGame == null);
            _activeGame = _getConnect4Game(match);

            _activeGame.OnGameEnded += GameHasEnded;
            _activeGame.OnGameStarted += GameStarted;
            _activeGame.OnMovePlayed += MovePlayed;

            _activeGame.Initialize();
        }
        private async void GameHasEnded(GameResult gameResult)
        {
            if (_activeGame == null)
            {
                Debug.Assert(false);
                return;
            }

            await _gameResultsService.Add(gameResult);

            _activeGame.OnGameEnded -= GameHasEnded;
            _activeGame.OnGameStarted -= GameStarted;
            _activeGame.OnMovePlayed -= MovePlayed;

            _activeGame.Match.Player1.Matching = null;
            _activeGame.Match.Player2.Matching = null;

            _activeGame.Match.Player1.IsInGame = false;
            _activeGame.Match.Player2.IsInGame = false;

            _activeGame.Dispose();
            _activeGame = null;

            Match? match;
            if (!_gamePlan.TryDequeue(out match))
            {
                Debug.Assert(false);
                return;
            }

            GameEnded(gameResult);

            TryStartGame();
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

        private Game? _activeGame = null;
        private readonly GameResultsService _gameResultsService;
        private readonly Func<Match, Game> _getConnect4Game;
        private ConcurrentQueue<Match> _gamePlan = new ConcurrentQueue<Match>();
    }
}
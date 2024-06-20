using backend.communication.mqtt;
using backend.game;
using backend.game.entities;
using backend.Infrastructure;
using backend.utilities;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace backend.services
{
    internal class GameManager : DisposingObject
    {
        public GameManager(
            Func<Match, Game> getConnect4Game,
            GameResultsService gameResultsService,
            RoboterAPI roboterAPI
            )
        {
            _getConnect4Game = getConnect4Game;
            _gameResultsService = gameResultsService;
            _roboterAPI = roboterAPI;

            _roboterAPI.OnManualMove += PlayManualMove;
            _roboterAPI.OnPlacingStone += PlacingStone;
        }


        public event Action<Player, Player>? OnRequestedMatch;
        public event Action<Player, Player>? OnRejectedMatch;
        public event Action<Match>? OnMatched;
        public event Action<Game>? OnGameStarted;
        public event Action<Player>? OnConfirmedGameStart;
        public event Action<Player, Field>? OnMovePlayed;
        public event Action<Player, int>? OnSendHint;
        public event Action<GameResult>? OnGameEnded;
        public event Action<Player, Field>? OnPlacingStone;

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
            if (requester.MatchingRequests.Contains(opponent))
                return false;

            opponent.MatchingRequests.Add(requester);
            RequestedMatch(requester, opponent);
            return true;
        }
        public void AcceptMatch(Player accepter, Player opponent)
        {
            accepter.MatchingRequests.Remove(opponent);
            accepter.Matching = opponent;
            opponent.Matching = accepter;

            Match match = new(accepter, opponent);
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

            Game.ConnfirmGameStart(player);
            ConfirmedGameStart(player);
        }
        public void PlayMove(Player player, int column)
        {
            if (_activeGame == null)
                throw new RequestErrorException();

            if (column < 0 || column > 6)
            {
                Debug.Assert(false);
                return;
            }

            _activeGame.PlayMove(player, column);
        }
        private void PlacingStone(Player player, Field field)
        {
            OnPlacingStone?.Invoke(player, field);
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

                List<Match> gamePlan = new(_gamePlan);
                gamePlan.Remove(foundMatch);
                _gamePlan = new ConcurrentQueue<Match>(gamePlan);
            }
        }

        protected override void OnDispose()
        {
            _roboterAPI.OnManualMove -= PlayManualMove;
            _roboterAPI.OnPlacingStone -= OnPlacingStone;
        }

        private void RequestedMatch(Player requester, Player opponent)
        {
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

        private void PlayManualMove(int column)
        {
            _activeGame?.PlayManualMove(column);
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
            await _gameResultsService.Add(gameResult);

            if (_activeGame == null)
            {
                Debug.Assert(false);
                return;
            }

            _activeGame.OnGameEnded -= GameHasEnded;
            _activeGame.OnGameStarted -= GameStarted;
            _activeGame.OnMovePlayed -= MovePlayed;

            _activeGame.Match.Player1.Matching = null;
            _activeGame.Match.Player2.Matching = null;

            _activeGame.Match.Player1.IsInGame = false;
            _activeGame.Match.Player2.IsInGame = false;

            _activeGame.Dispose();
            _activeGame = null;

            _gamePlan.TryDequeue(out Match? match);

            GameEnded(gameResult);
            TryStartGame();
        }
        private void TryStartGame()
        {
            if (_activeGame != null)
                return;

            if (!_gamePlan.TryPeek(out Match? match))
                return;

            StartNewGame(match);
        }

        private Game? _activeGame = null;
        private readonly GameResultsService _gameResultsService;
        private readonly RoboterAPI _roboterAPI;
        private readonly Func<Match, Game> _getConnect4Game;
        private ConcurrentQueue<Match> _gamePlan = new();
    }
}
using backend.communication.mqtt;
using backend.game;
using backend.game.entities;
using backend.Infrastructure;
using backend.services.player;
using backend.utilities;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection.PortableExecutable;

namespace backend.services
{
    internal class GameManager : DisposingObject
    {
        public GameManager(
            Func<Match, Game> getConnect4Game,
            GameResultsService gameResultsService,
            RoboterAPI roboterAPI,
            PlayerConnectionService playerConnectionService
            )
        {
            _getConnect4Game = getConnect4Game;
            _gameResultsService = gameResultsService;
            _roboterAPI = roboterAPI;
            _playerConnectionService = playerConnectionService;

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


        public void RequestMatch(Player requester, Player opponent)
        {
            if (requester.Matching != null)
                throw new InvalidPlayerRequestException($"Matcing request exception. Requester {requester.Username} has already matched with {requester.Matching.Username}.");

            if (opponent.Matching != null)
                throw new InvalidPlayerRequestException($"Matcing request exception. Opponent {opponent.Username} has already matched with {opponent.Matching.Username}.");

            _playerConnectionService.ForeachConnectedPlayer(p =>
            {
                if (p.Equals(requester))
                    return;

                if (p.MatchingRequests.Contains(requester))
                    throw new InvalidPlayerRequestException($"Matcing request exception. {requester.Username} has already requested {p.Username}.");
            });

            opponent.MatchingRequests.Add(requester);
            RequestedMatch(requester, opponent);
        }
        public void AcceptMatch(Player accepter, Player opponent)
        {
            if (!accepter.MatchingRequests.Contains(opponent))
                throw new InvalidPlayerRequestException($"Accept macht exception [accepter:{accepter.Username} opponent:{opponent.Username}]. There is no matching request to accept.");

            if (accepter.Matching != null)
                throw new InvalidPlayerRequestException($"Accept macht exception [accepter:{accepter.Username} opponent:{opponent.Username}]. Accepting player already has matched with {accepter.Matching.Username}.");


            if (opponent.Matching != null)
                throw new InvalidPlayerRequestException($"Accept macht exception [accepter:{accepter.Username} opponent:{opponent.Username}]. Opponent player already has matched with {opponent.Matching.Username}.");

            Debug.Assert(accepter.MatchingRequests.Remove(opponent));
            accepter.Matching = opponent;
            opponent.Matching = accepter;

            Match match = new(accepter, opponent);
            Matched(match);
            _gamePlan.Enqueue(match);

            TryStartGame();
        }
        public void RejectMatch(Player rejecter, Player opponent)
        {
            if (!rejecter.MatchingRequests.Contains(rejecter))
                throw new InvalidPlayerRequestException($"Reject macht exception [rejecter:{rejecter.Username} opponent:{opponent.Username}]. There is no matching request to reject.");

            rejecter.MatchingRequests.Remove(opponent);
            RejectedMatch(rejecter, opponent);
        }
        public void ConfirmGameStart(Player player)
        {
            if (_activeGame == null && !(player is OpponentRoboterPlayer))
                throw new InvalidPlayerRequestException($"Confirm game start exception [player:{player.Username}]. There is no active game for the accepting player.");

            if (player.HasConfirmedGameStart)
                throw new InvalidPlayerRequestException($"Confirm game start exception [player:{player.Username}]. Has already confirmed game start.");

            player.HasConfirmedGameStart = true;
            _activeGame?.GameStartGotConfirmed();
            ConfirmedGameStart(player);
        }
        public void PlayMove(Player player, int column)
        {
            if (_activeGame == null)
                throw new InvalidPlayerRequestException($"Play move exception [player:{player.Username} column:{column}]. No game is active.");

            _activeGame.PlayMove(player, column);
        }
        private void PlacingStone(Player player, Field field)
        {
            OnPlacingStone?.Invoke(player, field);
        }
        public int GetBestMove(Player player)
        {
            if (_activeGame == null)
                throw new InvalidPlayerRequestException($"Get best move exception [player:{player.Username}]. No game is active.");

            return _activeGame.GetBestMove(player);
        }
        public void QuitGame(Player player)
        {
            if (_activeGame == null)
                throw new InvalidPlayerRequestException($"Quit game exception [player:{player.Username}]. No game is active.");

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
        private readonly PlayerConnectionService _playerConnectionService;
        private readonly Func<Match, Game> _getConnect4Game;
        private ConcurrentQueue<Match> _gamePlan = new();
    }
}
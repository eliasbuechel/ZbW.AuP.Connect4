using backend.communication.mqtt;
using backend.game;
using backend.game.entities;
using backend.infrastructure;
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
            RoboterApi roboterAPI,
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

                if (p.MatchingRequests.Any(x => x.Player.Equals(requester)))
                    throw new InvalidPlayerRequestException($"Matcing request exception. {requester.Username} has already requested {p.Username}.");
            });

            MatchRequest matchRequest = new(requester);
            opponent.MatchingRequests.Add(matchRequest);
            RequestedMatch(requester, opponent);
            StartMatchRequestTimeout(requester, opponent, matchRequest.Id);
        }

        private void StartMatchRequestTimeout(Player requester, Player opponent, string id)
        {
            Thread thread = new(async () =>
            {
                await Task.Delay(REQUEST_TIMEOUT_DURATION_IN_MS);
                if (opponent.MatchingRequests.Any(x => x.Id == id))
                    RejectedMatch(opponent, requester);
            });
            thread.Start();
        }

        public void AcceptMatch(Player accepter, Player opponent)
        {
            MatchRequest? matchRequest = accepter.MatchingRequests.FirstOrDefault(x => x.Player.Equals(opponent)) ?? throw new InvalidPlayerRequestException($"Accept macht exception [accepter:{accepter.Username} opponent:{opponent.Username}]. There is no matching request to accept.");

            if (accepter.Matching != null)
                throw new InvalidPlayerRequestException($"Accept macht exception [accepter:{accepter.Username} opponent:{opponent.Username}]. Accepting player already has matched with {accepter.Matching.Username}.");


            if (opponent.Matching != null)
                throw new InvalidPlayerRequestException($"Accept macht exception [accepter:{accepter.Username} opponent:{opponent.Username}]. Opponent player already has matched with {opponent.Matching.Username}.");

            accepter.MatchingRequests.Remove(matchRequest);
            accepter.Matching = opponent;
            opponent.Matching = accepter;

            Match match = new(accepter, opponent);
            Matched(match);
            _gamePlan.Enqueue(match);

            TryStartGame();
        }
        public void RejectMatch(Player rejecter, Player opponent)
        {
            if (!rejecter.MatchingRequests.Any(x => x.Player.Equals(opponent)))
                throw new InvalidPlayerRequestException($"Reject macht exception [rejecter:{rejecter.Username} opponent:{opponent.Username}]. There is no matching request to reject.");

            MatchRequest matchRequest = rejecter.MatchingRequests.First(x => x.Player.Equals(opponent));
            rejecter.MatchingRequests.Remove(matchRequest);
            RejectedMatch(rejecter, opponent);
        }
        public void ConfirmGameStart(Player player)
        {
            if (_activeGame == null && player is not OpponentRoboterPlayer)
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
            MatchRequest? matchRequest = rejecter.MatchingRequests.FirstOrDefault(x => x.Player.Equals(opponent));
            if (matchRequest != null)
                opponent.MatchingRequests.Remove(matchRequest);

            OnRejectedMatch?.Invoke(rejecter, opponent);
        }
        private void Matched(Match match)
        {
            MatchRequest? matchRequestPlayer1 = match.Player1.MatchingRequests.FirstOrDefault(x => x.Player.Equals(match.Player2));
            if (matchRequestPlayer1 != null)
                match.Player1.MatchingRequests.Remove(matchRequestPlayer1);
            MatchRequest? matchRequestPlayer2 = match.Player2.MatchingRequests.FirstOrDefault(x => x.Player.Equals(match.Player1));
            if (matchRequestPlayer2 != null)
                match.Player1.MatchingRequests.Remove(matchRequestPlayer2);

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
        private Task MovePlayed(Player player, Field field)
        {
            Logger.Log(LogLevel.Debug, LogContext.GAME_PLAY, $"Invoke playing move in gameManager. Player: {player.Username} Column: {field.Column}");
            OnMovePlayed?.Invoke(player, field);
            return Task.CompletedTask;
        }
        private void SendHint(Player player, int column)
        {
            OnSendHint?.Invoke(player, column);
        }

        private void PlayManualMove(int column)
        {
            if (_activeGame == null)
                throw new InvalidPlayerRequestException("Cannot play manual move if there is no game running.");

            _activeGame.PlayManualMove(column);
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
            Logger.Log(LogLevel.Debug, LogContext.GAME_PLAY, $"Unregister from OnMovePlayed event of game.");
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
        private readonly RoboterApi _roboterAPI;
        private readonly PlayerConnectionService _playerConnectionService;
        private readonly Func<Match, Game> _getConnect4Game;
        private ConcurrentQueue<Match> _gamePlan = new();
        private const int REQUEST_TIMEOUT_DURATION_IN_MS = 15000;
    }
}
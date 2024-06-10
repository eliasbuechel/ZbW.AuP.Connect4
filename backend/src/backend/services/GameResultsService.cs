using backend.communication.DOTs;
using backend.communication.signalR.frontendApi;
using backend.Data;
using backend.Data.entities;
using backend.game;
using backend.game.entities;
using backend.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace backend.services
{
    internal class GameResultsService : DisposingObject
    {
        public GameResultsService(BackendDbContextFacory dbContextFactory, FrontendApi frontendApi)
        {
            _dbContextFactory = dbContextFactory;
            _frontendApi = frontendApi;

            _frontendApi.OnGetBestlist += OnGetBestlist;
        }

        public IEnumerable<GameResult> Bestlist
        {
            get
            {
                using BackendDbContext context = _dbContextFactory.GetDbContext();
                IEnumerable<GameResult> gameResults = context.GameResults
                    .Include(x => x.Line)
                    .Include(x => x.Match)
                    .Include(x => x.Match.Player1)
                    .Include(x => x.Match.Player2)
                    .Select(x => new GameResult(x))
                    .ToArray();
                return gameResults;
            }
        }
        public IEnumerable<GameResult> GameResults
        {
            get
            {
                using BackendDbContext context = _dbContextFactory.GetDbContext();
                IEnumerable<GameResult> gameResults = context.GameResults
                    .Include(x => x.Line)
                    .Include(x => x.Match)
                    .Include(x => x.Match.Player1)
                    .Include(x => x.Match.Player2)
                    .Select(x => new GameResult(x))
                    .ToArray();
                return gameResults;
            }
        }

        public async Task Add(GameResult gameResult)
        {
            using BackendDbContext context = _dbContextFactory.GetDbContext();
            DbGameResult dbGameResult = new DbGameResult();

            dbGameResult.Id = gameResult.Id;
            dbGameResult.WinnerId = gameResult.WinnerId;
            dbGameResult.Line = gameResult.Line == null ? new List<DbField>() : gameResult.Line.Select(x => new DbField(x)).ToList();
            dbGameResult.PlayedMoves = gameResult.PlayedMoves.ToList();
            dbGameResult.StartingPlayerId = gameResult.StartingPlayerId;

            DbGameResultMatch dbMatch = new DbGameResultMatch();
            DbPlayerInfo player1 = context.Players.FirstOrDefault(x => x.Id == gameResult.Match.Player1.Id) ?? new DbPlayerInfo(gameResult.Match.Player1);
            DbPlayerInfo player2 = context.Players.FirstOrDefault(x => x.Id == gameResult.Match.Player2.Id) ?? new DbPlayerInfo(gameResult.Match.Player2);
            dbMatch.Id = gameResult.Match.Id;
            dbMatch.Player1 = player1;
            dbMatch.Player2 = player2;
            dbGameResult.Match = dbMatch;

            context.Add(dbGameResult);
            await context.SaveChangesAsync();
        }

        protected override void OnDispose()
        {
            _frontendApi.OnGetBestlist -= OnGetBestlist;
        }

        private async void OnGetBestlist(PlayerIdentity playerIdentity, string connectionId)
        {
            IEnumerable<GameResultDTO> bestlistDTO = Bestlist.Select(x => new GameResultDTO(x));
            await _frontendApi.SendBestList(connectionId, bestlistDTO);
        }
        private void UpdateFrontend(Player player)
        {
        }

        private readonly FrontendApi _frontendApi;
        private readonly BackendDbContextFacory _dbContextFactory;
    }
}
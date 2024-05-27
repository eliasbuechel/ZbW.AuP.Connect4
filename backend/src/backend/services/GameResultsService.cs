using backend.Data;
using backend.Data.entities;
using backend.game.entities;
using Microsoft.EntityFrameworkCore;

namespace backend.services
{
    internal class GameResultsService
    {
        public GameResultsService(BackendDbContextFacory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
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
                    .Take(3)
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
            dbGameResult.TotalGameTime = gameResult.TotalGameTime;

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

        private readonly BackendDbContextFacory _dbContextFactory;
    }
}
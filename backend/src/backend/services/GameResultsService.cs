using backend.Data;
using backend.Data.entities;
using backend.game.entities;
using Microsoft.EntityFrameworkCore;

namespace backend.services
{
    internal class GameResultsService(BackendDbContextFacory dbContextFactory)
    {
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
                    .Include(x => x.PlayedMoves)
                    .Where(x => x.WinnerId != null)
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
                    .Include(x => x.PlayedMoves)
                    .Select(x => new GameResult(x))
                    .ToArray();
                return gameResults;
            }
        }

        public async Task Add(GameResult gameResult)
        {
            using BackendDbContext context = _dbContextFactory.GetDbContext();
            DbGameResult dbGameResult = new()
            {
                Id = gameResult.Id,
                WinnerId = gameResult.WinnerId,
                Line = GetOrAdd(gameResult.Line, context),
                PlayedMoves = GetOrAdd(gameResult.PlayedMoves, context),
                StartingPlayerId = gameResult.StartingPlayerId,
                Match = GetOrAdd(gameResult.Match, context)
            };

            context.GameResults.Add(dbGameResult);
            await context.SaveChangesAsync();
        }

        private List<DbPlayedMove> GetOrAdd(ICollection<PlayedMove> playedMoves, BackendDbContext context)
        {
            List<DbPlayedMove> dbPlayedMoves = [];

            for (int i = 0; i < playedMoves.Count; i++)
            {
                PlayedMove playedMove = playedMoves.ElementAt(i);

                dbPlayedMoves.Add(new DbPlayedMove()
                {
                    Id = Guid.NewGuid().ToString(),
                    Column = playedMove.Column,
                    MoveOrderIndex = i,
                    Duration = playedMove.Duration
                });
            }

            return dbPlayedMoves;
        }

        private List<DbField> GetOrAdd(ICollection<Field>? line, BackendDbContext context)
        {
            List<DbField> dbLine = [];

            if (line == null)
                return dbLine;

            foreach (var field in line)
                dbLine.Add(GetOrAdd(field, context));

            return dbLine;
        }

        private static DbField GetOrAdd(Field field, BackendDbContext context)
        {
            DbField? dbField = context.Fields.Where(x => x.Column == field.Column && x.Row == field.Row).FirstOrDefault();

            dbField ??= new DbField()
                {
                    Id = Guid.NewGuid().ToString(),
                    Column = field.Column,
                    Row = field.Row
                };

            return dbField;
        }

        private static DbPlayerInfo GetOrAdd(PlayerInfo player, BackendDbContext context)
        {
            DbPlayerInfo? dbPlayer = context.Players.FirstOrDefault(x => x.Id == player.Id);

            dbPlayer ??= new DbPlayerInfo
                {
                    Id = player.Id,
                    Username = player.Username
                };

            return dbPlayer;
        }
        private static DbGameResultMatch GetOrAdd(GameResultMatch match, BackendDbContext context)
        {
            DbPlayerInfo dbPlayer1 = GetOrAdd(match.Player1, context);
            DbPlayerInfo dbPlayer2 = GetOrAdd(match.Player2, context);

            DbGameResultMatch? dbGameResultMatch = context.Matches.Where(x => (x.Player1 == dbPlayer1 && x.Player2 == dbPlayer2) || (x.Player1 == dbPlayer2 && x.Player2 == dbPlayer1)).FirstOrDefault();

            dbGameResultMatch ??= new DbGameResultMatch()
                {
                    Id = Guid.NewGuid().ToString(),
                    Player1 = dbPlayer1,
                    Player2 = dbPlayer2
                };

            return dbGameResultMatch;
        }


        private readonly BackendDbContextFacory _dbContextFactory = dbContextFactory;
    }
}
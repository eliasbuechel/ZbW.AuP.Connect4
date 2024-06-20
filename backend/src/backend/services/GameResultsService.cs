﻿using backend.Data;
using backend.Data.entities;
using backend.game.entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
                var gameResultsQuery = context.GameResults
                    .Include(x => x.Line)
                    .Include(x => x.Match)
                    .Include(x => x.Match.Player1)
                    .Include(x => x.Match.Player2)
                    .Include(x => x.PlayedMoves)
                    .AsEnumerable();

                IEnumerable<GameResult> gameResults = gameResultsQuery
                    .Where(x => x.HasWinnerRow == true)
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
                    .Include(x => x.HasWinnerRow)
                    .Select(x => new GameResult(x))
                    .ToArray();
                return gameResults;
            }
        }

        public async Task Add(GameResult gameResult)
        {
            using BackendDbContext context = _dbContextFactory.GetDbContext();
            DbGameResult dbGameResult = new DbGameResult()
            {
                Id = gameResult.Id,
                WinnerId = gameResult.WinnerId,
                Line = GetOrAdd(gameResult.Line, context),
                PlayedMoves = GetOrAdd(gameResult.PlayedMoves, context),
                StartingPlayerId = gameResult.StartingPlayerId,
                Match = GetOrAdd(gameResult.Match, context),
                HasWinnerRow = gameResult.HasWinnerRow
            };

            context.GameResults.Add(dbGameResult);
            await context.SaveChangesAsync();
        }

        private IList<DbPlayedMove> GetOrAdd(ICollection<PlayedMove> playedMoves, BackendDbContext context)
        {
            IList<DbPlayedMove> dbPlayedMoves = new List<DbPlayedMove>();

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

        private IList<DbField> GetOrAdd(ICollection<Field>? line, BackendDbContext context)
        {
            IList<DbField> dbLine = new List<DbField>();

            if (line == null)
                return dbLine;

            foreach (var field in line)
                dbLine.Add(GetOrAdd(field, context));

            return dbLine;
        }

        private DbField GetOrAdd(Field field, BackendDbContext context)
        {
            DbField? dbField = context.Fields.Where(x => x.Column == field.Column && x.Row == field.Row).FirstOrDefault();

            if (dbField == null)
                dbField = new DbField()
                {
                    Id = Guid.NewGuid().ToString(),
                    Column = field.Column,
                    Row = field.Row
                };

            return dbField;
        }

        private DbPlayerInfo GetOrAdd(PlayerInfo player, BackendDbContext context)
        {
            DbPlayerInfo? dbPlayer = context.Players.FirstOrDefault(x => x.Id == player.Id);

            if (dbPlayer == null)
                dbPlayer = new DbPlayerInfo
                {
                    Id = player.Id,
                    Username = player.Username
                };

            return dbPlayer;
        }
        private DbGameResultMatch GetOrAdd(GameResultMatch match, BackendDbContext context)
        {
            DbPlayerInfo dbPlayer1 = GetOrAdd(match.Player1, context);
            DbPlayerInfo dbPlayer2 = GetOrAdd(match.Player2, context);

            DbGameResultMatch? dbGameResultMatch = context.Matches.Where(x => (x.Player1 == dbPlayer1 && x.Player2 == dbPlayer2) || (x.Player1 == dbPlayer2 && x.Player2 == dbPlayer1)).FirstOrDefault();

            if (dbGameResultMatch == null)
                dbGameResultMatch = new DbGameResultMatch()
                {
                    Id = Guid.NewGuid().ToString(),
                    Player1 = dbPlayer1,
                    Player2 = dbPlayer2
                };

            return dbGameResultMatch;
        }


        private readonly BackendDbContextFacory _dbContextFactory;
    }
}
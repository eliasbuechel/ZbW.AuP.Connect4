﻿using backend.data.entities;

namespace backend.game.entities
{
    internal class GameResult : Entity
    {
        public GameResult(Player? winner, ICollection<Field>? line, ICollection<PlayedMove> playedMoves, Player startingPlayer, Match match)
        {
            WinnerId = winner?.Id;
            Line = line;
            PlayedMoves = playedMoves;
            StartingPlayerId = startingPlayer.Id;
            Match = new GameResultMatch(match);
        }
        public GameResult(DbGameResult gameResult) : base(gameResult)
        {
            WinnerId = gameResult.WinnerId;
            Line = gameResult.Line.Count == 0 ? null : gameResult.Line.Select(x => new Field(x)).ToArray();
            PlayedMoves = gameResult.PlayedMoves.OrderBy(x => x.MoveOrderIndex).Select(x => new PlayedMove(x)).ToList();
            StartingPlayerId = gameResult.StartingPlayerId;
            Match = new GameResultMatch(gameResult.Match);
        }

        public string? WinnerId { get; }
        public ICollection<Field>? Line { get; }
        public ICollection<PlayedMove> PlayedMoves { get; }
        public string StartingPlayerId { get; }
        public GameResultMatch Match { get; }
    }
}

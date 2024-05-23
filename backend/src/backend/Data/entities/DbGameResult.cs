using backend.game.entities;
using System.Reflection;

namespace backend.Data.entities
{
    internal class DbGameResult : DbEntity
    {
        public DbGameResult() { }
        public DbGameResult(GameResult gameResult)
        {
            Id = gameResult.Id;
            WinnerId = gameResult.WinnerId;
            Line = gameResult.Line == null ? new List<DbField>() : gameResult.Line.Select(x => new DbField(x)).ToList();
            PlayedMoves = gameResult.PlayedMoves.ToList();
            StartingPlayerId = gameResult.StartingPlayerId;
            Match = new DbGameResultMatch(gameResult.Match);
        }

        public string? WinnerId { get; set; } = null;
        public virtual IList<DbField> Line { get; set; } = new List<DbField>();
        public virtual IList<int> PlayedMoves { get; set; } = new List<int>();
        public string StartingPlayerId { get; set; } = string.Empty;
        public virtual DbGameResultMatch Match { get; set; } = new DbGameResultMatch();
    }
}

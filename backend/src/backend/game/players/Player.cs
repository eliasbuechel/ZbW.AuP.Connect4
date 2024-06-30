using backend.game.entities;

namespace backend.game.players
{
    internal abstract class Player(string playerId, string username) : IEquatable<Player>
    {
        public string Id { get; } = playerId;
        public string Username { get; } = username;
        public bool HasConfirmedGameStart { get; set; }
        public bool IsInGame { get; set; }
        public TimeSpan? TotalPlayTime { get; set; }

        public readonly ICollection<MatchRequest> MatchingRequests = new List<MatchRequest>();
        public Player? Matching { get; set; }

        public bool Equals(Player? other)
        {
            return other != null
                && Id == other.Id;
        }
        public override bool Equals(object? obj)
        {
            return obj is Player player && Equals(player);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
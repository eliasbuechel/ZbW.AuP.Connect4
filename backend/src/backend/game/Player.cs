namespace backend.game
{
    internal abstract class Player(string playerId, string username) : IEquatable<Player>
    {
        public string Id { get; } = playerId;
        public string Username { get; } = username;
        public bool HasConfirmedGameStart { get; set; }
        public bool IsInGame { get; set; }

        public readonly ICollection<Player> MatchingRequests = new List<Player>();
        public Player? Matching { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Player player &&
                   Id == player.Id;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
        public bool Equals(Player? other)
        {
            return other != null
                && Id == other.Id;
        }
    }
}

namespace backend.game
{
    internal abstract class Player
    {
        public Player(string playerId, string username)
        {
            Id = playerId;
            Username = username;
        }

        public string Id { get; }
        public string Username { get; }
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
    }
}

using backend.communication.DOTs;
using backend.game.entities;
using backend.services;
using System.Diagnostics;

namespace backend.game
{
    internal abstract class Player : IEquatable<Player>
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

        public TimeSpan TotalPlayTime => _gameManager.GetTotalPlayTime(this);

        public event Action<IPlayer, int>? OnMovePlayed;
        public event Action<IPlayer, IPlayer>? OnMatch;
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

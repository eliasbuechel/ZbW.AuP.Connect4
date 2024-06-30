using backend.game.players;

namespace backend.game.entities
{
    internal class MatchRequest(Player player)
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        public Player Player { get; } = player;
    }
}
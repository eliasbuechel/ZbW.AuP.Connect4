using backend.game.players;

namespace backend.game.entities
{
    internal class Match(Player player1, Player player2) : Entity
    {
        public Player Player1 { get; } = player1;
        public Player Player2 { get; } = player2;
    }
}

using backend.data;

namespace backend.game
{
    internal class WebPlayer(PlayerIdentity playerIdentity) : Player(playerIdentity.Id, playerIdentity.UserName ?? "User with no username")
    {
        public int HintsLeft => _hintsLeft;
        public int? CurrentHint => _currentHint;

        public bool IsWatchingGame { get; set; }

        private readonly int _hintsLeft = MAX_HINTS;
        private readonly int? _currentHint = null;

        private const int MAX_HINTS = 2;
    }
}

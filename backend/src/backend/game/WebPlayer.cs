using backend.Data;

namespace backend.game
{
    internal class WebPlayer : Player
    {
        public WebPlayer(PlayerIdentity playerIdentity) : base(playerIdentity.Id, playerIdentity.UserName ?? "User with no username")
        { }

        public int HintsLeft => _hintsLeft;
        public int? CurrentHint => _currentHint;

        public bool IsWatchingGame { get; set; }

        private int _hintsLeft = MAX_HINTS;
        private int? _currentHint = null;
        private const int MAX_HINTS = 3;
    }
}

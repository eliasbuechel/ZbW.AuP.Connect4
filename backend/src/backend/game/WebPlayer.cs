using backend.Data;

namespace backend.game
{
    internal class WebPlayer : Player
    {
        public WebPlayer(PlayerIdentity playerIdentity) : base(playerIdentity.Id, playerIdentity.UserName ?? "User with no username")
        { }

        public int HintsLeft => _hintsLeft;
        public int? CurrentHint => _currentHint;

        public bool WatchingGame { get; set; }

        private int _hintsLeft = MAX_HINTS;
        private int? _currentHint = null;
        private const int MAX_HINTS = 3;

    }

    //internal class WebPlayer : Player
    //{
    //    public WebPlayer(PlayerIdentity playerIdentity, GameManager gameManager) : base(playerIdentity.Id, playerIdentity.UserName ?? "User with no username", gameManager)
    //    { }

    //    public int HintsLeft => _hintsLeft;
    //    public int? CurrentHint => _currentHint;

    //    protected override void OnRequestedMatch(Player requester, Player opponent)
    //    {
    //        throw new NotImplementedException();
    //    }
    //    protected override void OnRejectedMatch(Player rejecter, Player opponent)
    //    {
    //        throw new NotImplementedException();
    //    }
    //    protected override void OnMatched(Match match)
    //    {
    //        throw new NotImplementedException();
    //    }
    //    protected override void OnGameStarted(Game game)
    //    {
    //        throw new NotImplementedException();
    //    }
    //    protected override void OnConfirmedGameStart(Player player)
    //    {
    //        throw new NotImplementedException();
    //    }
    //    protected override void OnMovePlayed(Player player, Field field)
    //    {
    //        throw new NotImplementedException();
    //    }
    //    protected override void OnQuitGame(Player player)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private int _hintsLeft = MAX_HINTS;
    //    private int? _currentHint = null;
    //    private const int MAX_HINTS = 3;

    //}
}

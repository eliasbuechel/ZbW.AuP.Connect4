namespace backend.game
{
    internal interface IPlayer
    {
        event Action<IPlayer, int>? OnMovePlayed;
        event Action<IPlayer, IPlayer>? OnMatch;

        string Id { get; }
        string Username { get; }
        bool HasConfirmedGameStart { get; set; }
        public int HintsLeft { get; }
        public int? CurrentHint { get; }
        public IEnumerable<string> Connections { get; }

        void ConfirmGameStart();
        void RequestMatch(IPlayer player);
        void RequestedMatch(IPlayer player);
        void RejectedMatch(IPlayer opponent);
        void PlayerConnected(IPlayer player);
        void PlayerDisconnected(IPlayer player);
        IEnumerable<IPlayer> GetOnlinePlayers();
        void Connect(string connection);
        void Disconnected(string connection);
        bool HasRequestedMatch(IPlayer you);
        bool HasMatched(IPlayer player);
        void AcceptMatch(IPlayer player);
        void Matched(Match match);
        void MatchingEnded(Match match);
        Task RejectMatch(IPlayer player);
        IEnumerable<Match> GetGamePlan();
        void MovePlayed(IPlayer player, Field field);
        void PlayMove(int column);
        void GameStarted(Connect4Game connect4Game);
        Connect4Game GetCurrentGameState();
        void QuitGame();
        void GameEnded(GameResult gameResult);
        Task GetGame(string connection);
        Task GetUserDataAsync(string connection);
        Task GetOnlinePlayers(string connection);
        Task GetGamePlan(string connection);
        Task GetCurrentGame(string connection);
        void OpponentConfirmedGameStart();
        void GameStartConfirmed();
        void YouConfirmedGameStart();
        void GetHint();
    }
}
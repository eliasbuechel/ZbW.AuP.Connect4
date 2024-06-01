using backend.game.entities;

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
        public ICollection<string> Connections { get; }

        Task ConfirmGameStartAsync();
        Task RequestMatch(IPlayer player);
        void RequestedMatch(IPlayer player);
        void RejectedMatch(IPlayer opponent);
        void PlayerConnected(IPlayer player);
        void PlayerDisconnected(IPlayer player);
        bool HasRequestedMatch(IPlayer you);
        bool HasMatched(IPlayer player);
        Task AcceptMatchAsync(IPlayer player);
        void Matched(Match match);
        void MatchingEnded(Match match);
        Task RejectMatchAsync(IPlayer player);
        void MovePlayed(IPlayer player, Field field);
        Task PlayMoveAsync(int column);
        void GameStarted(Game connect4Game);
        Game GetCurrentGameState();
        Task QuitGameAsync();
        void GameEnded(GameResult gameResult);
        Task GetGameAsync(string connection);
        Task SendGame(Game game);
        Task GetUserDataAsync(string connection);
        Task GetOnlinePlayersAsync(string connection);
        Task GetGamePlanAsync(string connection);
        Task GetCurrentGameAsync(string connection);
        void GameStartConfirmed();
        void ConfirmedGameStart(IPlayer player);
        void GetHint();
        Task GetBestlist(string connection);
        public void SendBestList(IEnumerable<GameResult> bestlist);
    }
}
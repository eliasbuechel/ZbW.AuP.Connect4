using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace backend.game
{
    internal interface IPlayer
    {
        event Action<IPlayer, int>? OnMovePlayed;
        event Action<IPlayer, IPlayer>? OnMatch;

        string Id { get; }
        string Username { get; }
        bool HasConfirmedGameStart { get; }
        public IEnumerable<string> Connections { get; }

        void ConfirmGameStart();
        void RequestMatch(IPlayer player);
        void RequestedMatch(IPlayer player);
        void RejectedMatch(IPlayer opponent);
        void PlayerConnected(IPlayer player);
        void PlayerDisconnected(IPlayer player);
        IEnumerable<IPlayer> GetOnlinePlayers();
        void Connected(string connectionId);
        void Disconnected(string connectionId);
        bool HasRequestedMatch(IPlayer you);
        bool HasMatched(IPlayer player);
        void AcceptMatch(IPlayer player);
        void Matched(Match match);
        void RejectMatch(IPlayer player);
        IEnumerable<Match> GetGamePlan();
        void MovePlayed(int column);
        void PlayMove(int column);
        void GameStarted();
        Connect4Game GetCurrentGameState();
        void QuitGame();
        void GameEnded(GameResult gameResult);
        bool HasGameStarted();
    }
}
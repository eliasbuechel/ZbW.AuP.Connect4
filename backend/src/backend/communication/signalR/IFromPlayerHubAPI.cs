using backend.game;

namespace backend.communication.signalR
{
    internal interface IFromPlayerHubAPI
    {
        void GetUserData();
        void GetOnlinePlayers();
        void GetGamePlan();
        void GetCurrentGame();
        void RequestMatch();
        void AcceptMatch();
        void RejectMatch();
        void PlayMove(int column);
        void QuitGame();
        public void HasGameStarted();
    }

    internal interface IToPlayerHubAPI
    {
        void PlayerConnected(IPlayer player);
        void PlayerDisconnected(IPlayer player);
        void RequestedMatch(IPlayer player);
        void RejectedMatch(IPlayer player);
        void Matched(Match match);
        void MovePlayed(IPlayer player, Field field);
        void GameStarted(Connect4Game connect4Game);
        void GameEnded(GameResult gameResult);
    }
}

using backend.communication.DOTs;
using backend.game;
using Microsoft.AspNetCore.SignalR;

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
}

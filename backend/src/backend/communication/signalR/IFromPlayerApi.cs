namespace backend.communication.signalR
{
    internal interface IFromPlayerApi
    {
        public void GetGamePlan();
        public void GetGame();
        public void GetUserData();
        public void GetOnlinePlayers();
        public void GetCurrentGame();
        public void ConfirmGameStart();
        public void PlayMove(int column);
        public void QuitGame();
    }
}

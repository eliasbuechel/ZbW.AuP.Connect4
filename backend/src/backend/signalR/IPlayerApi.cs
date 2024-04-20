namespace backend.signalR
{
    internal interface IPlayerApi
    {
        public void MakeMove(int column);
        public void ConfirmGameStart();
    }
}
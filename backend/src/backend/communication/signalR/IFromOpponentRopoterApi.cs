namespace backend.communication.signalR
{
    internal interface IFromOpponentRopoterApi : IFromPlayerApi
    {
        public void RequestMatch();
        public void AcceptMatch();
        public void RejectMatch();
    }
}

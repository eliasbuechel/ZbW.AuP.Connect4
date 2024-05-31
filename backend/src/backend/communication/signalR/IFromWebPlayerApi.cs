namespace backend.communication.signalR
{
    internal interface IFromWebPlayerApi : IFromPlayerApi
    {
        public void RequestMatch(string playerId);
        public void AcceptMatch(string playerId);
        public void RejectMatch(string playerId);
        public void RequestSinglePlayerMatch();
        public void GetHint();
        public void GetBestlist();
    }
}
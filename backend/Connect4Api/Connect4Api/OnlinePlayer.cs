namespace Connect4Api
{
    public class OnlinePlayer
    {
        public string ConnectionId => _connectionId;

        public OnlinePlayer(string connectionId)
        {
            _connectionId = connectionId;
        }

        private string _connectionId;
    }
}
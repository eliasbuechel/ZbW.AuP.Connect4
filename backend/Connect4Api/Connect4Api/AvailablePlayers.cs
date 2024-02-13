using System.Data;
using System.Diagnostics;

namespace Connect4Api
{
    public class AvailablePlayers
    {
        public string[] ConnectionIds => availablePlayers.Select(op => op.ConnectionId).ToArray();

        public void Add(string connectionId)
        {
            Debug.Assert(!Contains(connectionId));

            if (Contains(connectionId))
                return;

            availablePlayers.Add(new OnlinePlayer(connectionId));
        }
        public void Remove(string connectionId)
        {
            OnlinePlayer connectedUser = availablePlayers.Where(c => c.ConnectionId == connectionId).First();
            availablePlayers.Remove(connectedUser);
        }

        private bool Contains(string _connectionId)
        {
            foreach (var connectedUser in availablePlayers)
                if (connectedUser.ConnectionId == _connectionId)
                    return true;

            return false;
        }
        
        private List<OnlinePlayer> availablePlayers = new List<OnlinePlayer>();
    }
}
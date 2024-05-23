using backend.game;
using backend.services;
using Microsoft.AspNetCore.SignalR;

namespace backend.communication.signalR
{
    internal abstract class PlayerHub : Hub
    {
        public PlayerHub(IOnlinePlayerProvider onlinePlayerProvider)
        {
            _onlinePlayerProvider = onlinePlayerProvider;
        }

        public void GetGamePlan()
        {
            lock (RequestLock)
            {
                ThisPlayer.GetGamePlan(Connection).Wait();
            }
        }
        public void GetGame()
        {
            lock (RequestLock)
            {
                ThisPlayer.GetGame(Connection).Wait();
            }
        }
        public void GetUserData()
        {
            lock (RequestLock)
            {
                ThisPlayer.GetUserDataAsync(Connection).Wait();
            }
        }
        public void GetOnlinePlayers()
        {
            lock (RequestLock)
            {
                ThisPlayer.GetOnlinePlayers(Connection).Wait();
            }
        }
        public void GetCurrentGame()
        {
            lock (RequestLock)
            {
                ThisPlayer.GetCurrentGame(Connection).Wait();
            }
        }
        public void RequestMatch(string playerId)
        {
            lock (RequestLock)
            {
                IPlayer player = _onlinePlayerProvider.GetOnlinePlayer(playerId);
                ThisPlayer.RequestMatch(player);
            }
        }
        public void AcceptMatch(string playerId)
        {
            lock (RequestLock)
            {
                IPlayer player = _onlinePlayerProvider.GetOnlinePlayer(playerId);
                ThisPlayer.AcceptMatch(player);
            }
        }
        public void RejectMatch(string playerId)
        {
            lock (RequestLock)
            {
                IPlayer player = _onlinePlayerProvider.GetOnlinePlayer(playerId);
                ThisPlayer.RejectMatch(player).Wait();
            }
        }
        public void ConfirmGameStart()
        {
            lock (RequestLock)
            {
                ThisPlayer.ConfirmGameStart();
            }
        }
        public void PlayMove(int column)
        {
            lock (RequestLock)
            {
                ThisPlayer.PlayMove(column);
            }
        }
        public void QuitGame()
        {
            lock (RequestLock)
            {
                ThisPlayer.QuitGame();
            }
        }

        public override Task OnConnectedAsync()
        {
            lock (RequestLock)
            {
                IPlayer player = GetOrCreatePlayer();
                player.Connect(Connection);
                return Task.CompletedTask;
            }
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            lock (RequestLock)
            {
                IPlayer? player = GetPlayerOrDefault();

                if (player != null)
                    ThisPlayer.Disconnected(Connection);

                return Task.CompletedTask;
            }
        }

        protected abstract IPlayer GetOrCreatePlayer();
        protected abstract IPlayer? GetPlayerOrDefault();

        protected string Connection => Context.ConnectionId;
        protected abstract IPlayer ThisPlayer { get; }
        protected abstract object RequestLock { get; }

        protected readonly IOnlinePlayerProvider _onlinePlayerProvider;
    }
}

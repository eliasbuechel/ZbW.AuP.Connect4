using backend.game;
using backend.services;

namespace backend.communication.signalR
{
    internal class OpponentRoboterPlayerHub : PlayerHub
    {
        public OpponentRoboterPlayerHub(IOnlinePlayerProvider onlinePlayerProvider, ToPlayerHub<OpponentRoboterPlayerHub> opponentRoboterPlayer) : base(onlinePlayerProvider)
        {
            _opponentRoboterPlayer = opponentRoboterPlayer;
        }

        protected override IPlayer ThisPlayer => _opponentRoboterPlayer;
        protected override object RequestLock => _requestLock;
        protected override IPlayer GetOrCreatePlayer()
        {
            return _opponentRoboterPlayer;
        }
        protected override IPlayer? GetPlayerOrDefault()
        {
            return _opponentRoboterPlayer;
        }

        private static object _requestLock = new object();
        private readonly ToPlayerHub<OpponentRoboterPlayerHub> _opponentRoboterPlayer;
    }
}

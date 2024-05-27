using backend.game;
using backend.services;

namespace backend.communication.signalR
{
    internal class OpponentRoboterPlayerHub : PlayerHub
    {
        public OpponentRoboterPlayerHub(IOnlinePlayerProvider onlinePlayerProvider, PlayerRequestHandlerManager playerRequestHandlerManager, ToPlayerHub<OpponentRoboterPlayerHub> opponentRoboterPlayer) : base(onlinePlayerProvider, playerRequestHandlerManager)
        {
            _opponentRoboterPlayer = opponentRoboterPlayer;
        }

        protected override IPlayer ThisPlayer => _opponentRoboterPlayer;
        protected override IPlayer GetOrCreatePlayer()
        {
            return _opponentRoboterPlayer;
        }
        protected override IPlayer? GetPlayerOrDefault()
        {
            return _opponentRoboterPlayer;
        }

        private readonly ToPlayerHub<OpponentRoboterPlayerHub> _opponentRoboterPlayer;
    }
}

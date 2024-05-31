using backend.game;
using backend.services;
using System.Diagnostics;

namespace backend.communication.signalR
{
    internal class OpponentRoboterPlayerHub : PlayerHub<OpponentRoboterPlayer, string>
    {
        public OpponentRoboterPlayerHub(
            PlayerRequestHandlerManager playerRequestHandlerManager,
            OpponentRoboterPlayerHubManager opponentRoboterPlayerManager,
            Func<string, OpponentRoboterPlayer> createOpponentRoboterPlayer,
            AlgorythmPlayerManager algorythmPlayerManager,
            Func<IPlayer, AlgorythmPlayer> createAlgorythmPlayer,
            ConnectedPlayerProvider connectedPlayerProvider
            ) : base(playerRequestHandlerManager, opponentRoboterPlayerManager, createOpponentRoboterPlayer, connectedPlayerProvider)
        {
            _algorythmPlayerManager = algorythmPlayerManager;
            _createAlgorythmPlayer = createAlgorythmPlayer;
        }

        public void RequestMatch()
        {
            OpponentRoboterPlayer player;
            try
            {
                player = ThisPlayer;
                RequestHandler.Enqueue(async () =>
                {
                    _algorythmPlayerManager.ConnectPlayer(player, _createAlgorythmPlayer);
                    AlgorythmPlayer algorythmPlayer = _algorythmPlayerManager.GetConnectedPlayerByIdentification(player);
                    await player.RequestMatch(algorythmPlayer);
                });
            }
            catch
            {
                Debug.Assert(false);
            }
        }
        public void AcceptMatch()
        {
            IPlayer player;
            try
            {
                player = ThisPlayer;
                RequestHandler.Enqueue(async () =>
                {
                    IPlayer algorythmPlayer = _algorythmPlayerManager.GetConnectedPlayerByIdentification(player);
                    await player.AcceptMatchAsync(algorythmPlayer);
                });
            }
            catch
            {
                Debug.Assert(false);
            }
        }
        public void RejectMatch()
        {
            IPlayer player;
            try
            {
                player = ThisPlayer;
                RequestHandler.Enqueue(async () =>
                {
                    IPlayer algorythmPlayer = _algorythmPlayerManager.GetConnectedPlayerByIdentification(player);
                    await player.RejectMatchAsync(algorythmPlayer);
                });
            }
            catch
            {
                Debug.Assert(false);
            }
        }

        protected override string Identification => Context.ConnectionId;

        private readonly AlgorythmPlayerManager _algorythmPlayerManager;
        private readonly Func<IPlayer, AlgorythmPlayer> _createAlgorythmPlayer;
    }
}
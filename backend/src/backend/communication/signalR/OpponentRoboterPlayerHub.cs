using backend.Data;
using backend.game;
using backend.services;
using System.Diagnostics;

namespace backend.communication.signalR
{
    internal class OpponentRoboterPlayerHub : PlayerHub
    {
        public OpponentRoboterPlayerHub(PlayerRequestHandlerManager playerRequestHandlerManager, ToPlayerHub<OpponentRoboterPlayerHub> opponentRoboterPlayer, AlgorythmPlayerProvider algorythmPlayerProvider, Func<AlgorythmPlayer> createAlgorythmPlayer) : base(playerRequestHandlerManager)
        {
            _opponentRoboterPlayer = opponentRoboterPlayer;
            _algorythmPlayerProvider = algorythmPlayerProvider;
            _createAlgorythmPlayer = createAlgorythmPlayer;
        }

        public void RequestMatch()
        {
            IPlayer player;
            try
            {
                player = ThisPlayer;
                RequestHandler.Enqueue(async () =>
                {
                    IPlayer algorythmPlayer = _algorythmPlayerProvider.CreateAlgorythmPlayer(player, _createAlgorythmPlayer);
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
                    IPlayer algorythmPlayer = _algorythmPlayerProvider.GetAlgorythmPlayer(player);
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
                    IPlayer algorythmPlayer = _algorythmPlayerProvider.GetAlgorythmPlayer(player);
                    await player.RejectMatchAsync(algorythmPlayer);
                });
            }
            catch
            {
                Debug.Assert(false);
            }
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
        private readonly AlgorythmPlayerProvider _algorythmPlayerProvider;
        private readonly Func<AlgorythmPlayer> _createAlgorythmPlayer;
    }
}

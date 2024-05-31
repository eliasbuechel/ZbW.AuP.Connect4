using backend.game;
using backend.services;
using Microsoft.AspNetCore.SignalR;

namespace backend.communication.signalR
{
    internal class OpponentRoboterPlayer : ToPlayerHub<OpponentRoboterPlayerHub>
    {
        public OpponentRoboterPlayer(string playerId, string username, GameManager gameManager, IHubContext<OpponentRoboterPlayerHub> hubContext) : base(playerId, username, gameManager, hubContext)
        { }
    }
}

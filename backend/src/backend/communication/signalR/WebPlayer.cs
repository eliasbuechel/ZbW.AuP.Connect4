using backend.game;
using backend.services;
using Microsoft.AspNetCore.SignalR;

namespace backend.communication.signalR
{
    internal class WebPlayer : ToPlayerHub<WebPlayerHub>
    {
        public WebPlayer(string playerId, string username, GameManager gameManager, IHubContext<WebPlayerHub> hubContext) : base(playerId, username, gameManager, hubContext)
        { }
    }
}
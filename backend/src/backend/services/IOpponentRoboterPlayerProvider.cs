using backend.communication.signalR;

namespace backend.services
{

    internal interface IOpponentRoboterPlayerProvider : IPlayerProvider<OpponentRoboterPlayerHubClient, string>
    { }
}
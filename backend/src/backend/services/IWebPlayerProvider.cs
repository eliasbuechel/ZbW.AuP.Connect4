using backend.communication.signalR;
using backend.Data;

namespace backend.services
{
    internal interface IWebPlayerProvider : IPlayerProvider<ToPlayerHub<WebPlayerHub>, PlayerIdentity>
    { }
}
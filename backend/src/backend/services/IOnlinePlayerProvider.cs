using backend.communication.signalR;
using backend.Data;
using backend.game;

namespace backend.services
{
    internal interface IOnlinePlayerProvider
    {
        IPlayer GetOnlinePlayerAsync(string playerId);
        IPlayer? GetOnlinePlayerOrDefault(string playerId);
        IPlayer GetOrCreatePlayer(PlayerIdentity playerIdentity, Func<PlayerIdentity, ToPlayerHub<WebPlayerHub>> createPlayer);
    }
}
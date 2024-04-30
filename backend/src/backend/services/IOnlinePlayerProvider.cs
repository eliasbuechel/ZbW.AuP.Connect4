using backend.database;
using backend.game;

namespace backend.services
{
    internal interface IOnlinePlayerProvider
    {
        IPlayer GetPlayer(PlayerIdentity identity);
        IPlayer GetPlayer(string playerId);
        IPlayer? GetPlayerOrDefault(PlayerIdentity identity);
    }
}

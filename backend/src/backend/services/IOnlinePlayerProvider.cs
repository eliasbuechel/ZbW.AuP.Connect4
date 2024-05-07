using backend.game;

namespace backend.services
{
    internal interface IOnlinePlayerProvider
    {
        IPlayer GetOnlinePlayer(string playerId);
        IPlayer? GetOnlinePlayerOrDefault(string playerId);
    }
}

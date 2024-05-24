using backend.game;

namespace backend.services
{
    internal interface IOnlinePlayerProvider
    {
        IPlayer GetOnlinePlayerAsync(string playerId);
        IPlayer? GetOnlinePlayerOrDefault(string playerId);
    }
}

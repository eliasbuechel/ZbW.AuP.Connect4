using backend.game;

namespace backend.services
{
    internal interface IPlayerProvider<TPlayer, TIdentification> where TPlayer : IPlayer
    {
        TPlayer GetConnectedPlayer(string playerId);
        TPlayer? GetConnectedPlayerOrDefault(string playerId);
    }
}
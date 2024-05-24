using backend.game;
using System.Collections.Concurrent;

namespace backend.communication.signalR
{
    internal class PlayerRequestHandlerManager
    {
        public PlayerRequestHandler GetOrCreateHandler(IPlayer player)
        {
            return _handlers.GetOrAdd(player, id => new PlayerRequestHandler());
        }
        public void RemoveHandler(IPlayer player)
        {
            if (_handlers.TryRemove(player, out var handler))
            {
                handler.Stop();
            }
        }

        private readonly ConcurrentDictionary<IPlayer, PlayerRequestHandler> _handlers = new ConcurrentDictionary<IPlayer, PlayerRequestHandler>();
    }
}

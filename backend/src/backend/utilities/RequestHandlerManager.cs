using System.Collections.Concurrent;

namespace backend.utilities
{
    internal class RequestHandlerManager<TRequestIdentification> where TRequestIdentification : notnull
    {
        public RequestHandler GetOrCreateHandler(TRequestIdentification requestIdentification)
        {
            return _handlers.GetOrAdd(requestIdentification, ri => new RequestHandler());
        }

        public void RemoveHandler(TRequestIdentification requestIdentification)
        {
            if (_handlers.TryRemove(requestIdentification, out var handler))
            {
                handler.Stop();
            }
        }

        private readonly ConcurrentDictionary<TRequestIdentification, RequestHandler> _handlers = new();
    }
}

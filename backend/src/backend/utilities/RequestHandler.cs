using backend.Infrastructure;
using System.Collections.Concurrent;

namespace backend.utilities
{
    internal class RequestHandler
    {
        public RequestHandler()
        {
            _workerThread = new Thread(ProcessRequests);
            _workerThread.Start();
        }

        public event Action<string>? OnRequestError;

        private async void ProcessRequests()
        {
            while (_running)
            {
                if (_queue.TryDequeue(out var request))
                    try
                    {
                        await request.Methode.Invoke();
                    }
                    catch (InvalidPlayerRequestException e)
                    {
                        OnRequestError?.Invoke(request.ConnectionId);
                        Logger.Log(LogCase.ERROR, e.Message);
                    }
                else
                    Thread.Sleep(100); // Avoid busy-waiting
            }
        }
        public void Stop()
        {
            _running = false;
            _workerThread.Join();
        }
        public void Enqueue(Func<Task> method, string connectionId)
        {
            _queue.Enqueue(new Request(method, connectionId));
        }

        private bool _running = true;
        private readonly Thread _workerThread;
        private readonly ConcurrentQueue<Request> _queue = new();
    }
}
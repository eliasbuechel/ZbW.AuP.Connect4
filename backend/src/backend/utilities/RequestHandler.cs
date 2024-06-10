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

        private async void ProcessRequests()
        {
            while (_running)
            {
                if (_queue.TryDequeue(out var request))
                    await request.Invoke();
                else
                    Thread.Sleep(100); // Avoid busy-waiting
            }
        }
        public void Stop()
        {
            _running = false;
            _workerThread.Join();
        }
        public void Enqueue(Func<Task> method)
        {
            _queue.Enqueue(method);
        }

        private bool _running = true;
        private readonly Thread _workerThread;
        private readonly ConcurrentQueue<Func<Task>> _queue = new ConcurrentQueue<Func<Task>>();
    }
}

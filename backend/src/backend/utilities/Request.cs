namespace backend.utilities
{
    internal class Request(Func<Task> methode, string connectionId)
    {
        public Func<Task> Methode { get; } = methode;
        public string ConnectionId { get; } = connectionId;
    }
}

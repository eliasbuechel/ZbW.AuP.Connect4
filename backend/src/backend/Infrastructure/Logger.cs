namespace backend.Infrastructure
{
    internal enum LogCase
    {
        DEBUG = 0,
        WARN = 1,
        ERROR = 2,
        FATAL = 3
    }

    internal static class Logger
    {
        public static void Log(LogCase logCase, string message)
        {
            Console.WriteLine($"{Convert.ToString(logCase)}: {message}");
        }

        public static void Log(LogCase logCase, string message, Exception exception)
        {
            Log(logCase, $"{message} INNER_MESSAGE: {exception.Message}");
        }
    }
}

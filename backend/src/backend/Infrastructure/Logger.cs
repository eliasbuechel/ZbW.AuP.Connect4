namespace backend.infrastructure
{
    internal enum LogContext
    {
        MQTT_CLIENT = 0,
        ENTITY_FRAMEWORK,
        OPPONENT_ROBOTER_CLIENT_API,
        OPPONENT_ROBOTER_HUB_API,
        PLAYER_REQUEST,
        GAME_PLAY,
        EMAIL_SENDER,
        CONNECTION_MANAGER,
        ROBOTER_API,
        ALGORYTHM_PLAYER
    }

    internal static class Logger
    {
        private static void Log(LogLevel logLevel, string message)
        {
            switch (logLevel)
            {
                case LogLevel.Information:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                default:
                    Console.ResetColor();
                    break;
            }
            Console.Write($"{Convert.ToString(logLevel)}: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{message}");
        }

        public static void Log(LogLevel logLevel, LogContext context, string message)
        {
            if (context == LogContext.ENTITY_FRAMEWORK && (logLevel == LogLevel.Information || logLevel == LogLevel.Debug))
                return;

            if (context == LogContext.MQTT_CLIENT)
                return;

            Log(logLevel, $"{Convert.ToString(context)}: {message}");
        }
        public static void Log(LogLevel logCase, LogContext context, string message, Exception exception)
        {
            Log(logCase, context, $"{message} DETAILS: {exception.Message}");
        }
    }
}

﻿namespace backend.Infrastructure
{
    internal enum LogContext
    {
        MQTT_CLIENT = 0,
        ENTITY_FRAMEWORK,
        OPPONENT_ROBOTER_CLIENT_API,
        OPPONENT_ROBOTER_HUB_API,
        PLAYER_REQUEST
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
            Log(logLevel, $"{Convert.ToString(context)}: {message}");
        }
        public static void Log(LogLevel logCase, LogContext context, string message, Exception exception)
        {
            Log(logCase, context, $"{message} INNER_MESSAGE: {exception.Message}");
        }
    }
}

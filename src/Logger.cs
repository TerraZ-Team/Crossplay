using System;

namespace Crossplay
{
    static class Logger
    {
        public static void LogMessage(string message, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"[Crossplay] {message}");
            Console.ResetColor();
        }
    }
}

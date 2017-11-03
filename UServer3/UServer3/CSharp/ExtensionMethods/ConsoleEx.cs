using System;

namespace UServer3.CSharp.ExtensionMethods
{
    public static class ConsoleEx
    {
        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop-1;
            Console.SetCursorPosition(0, Console.CursorTop-1);
            Console.Write(new string(' ', Console.WindowWidth)); 
            Console.SetCursorPosition(0, currentLineCursor);
        }
    }
}
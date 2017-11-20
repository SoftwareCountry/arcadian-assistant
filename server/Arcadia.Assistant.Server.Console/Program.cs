namespace Arcadia.Assistant.Server.Console
{
    using System;

    internal class Program
    {
        public static void Main(string[] args)
        {
            using (var app = new Application())
            {
                app.Start();
                Console.ReadKey();
            }
        }
    }
}

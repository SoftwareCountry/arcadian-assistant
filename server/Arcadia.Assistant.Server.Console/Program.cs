namespace Arcadia.Assistant.Server.Console
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class Program
    {
        private static readonly AutoResetEvent closing = new AutoResetEvent(false);

        public static void Main(string[] args)
        {
            //var config = new ConfigurationBuilder

            using (var app = new Application())
            {
                app.Start();
                Console.CancelKeyPress += OnExit;
                closing.WaitOne();
            }
        }

        private static void OnExit(object sender, ConsoleCancelEventArgs args)
        {
            Console.WriteLine("Exiting...");
            closing.Set();
        }
    }
}

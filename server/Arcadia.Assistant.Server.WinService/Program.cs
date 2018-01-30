namespace Arcadia.Assistant.Server.WinService
{
    using System;
    using System.IO;
    using System.ServiceProcess;

    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var servicesToRun = new ServiceBase[]
                {
                    new MainService()
                };
            ServiceBase.Run(servicesToRun);
        }
    }
}

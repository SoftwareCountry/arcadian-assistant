namespace Arcadia.Assistant.Server.Console
{
    using System;

    using Akka.Actor;
    using Arcadia.Assistant.Organization;

    internal class Program
    {
        public static void Main(string[] args)
        {
            using (var app = new Application())
            {
                app.Start();

                var employees = app.ActorSystem.ActorOf(Props.Create(() => new EmployeesActor()), "employees");
                var info = employees.Ask<EmployeeDemographics>(new RequestDemographics("1")).Result;

                Console.ReadKey();
            }
        }
    }
}

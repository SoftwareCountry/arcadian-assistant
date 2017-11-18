namespace Arcadia.Assistant.Organization
{
    using Akka.Actor;

    public class EmployeesActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case RequestDemographics request:
                    var employee = Context.Child(request.EmployeeId) is Nobody
                        ? Context.ActorOf(Props.Create(() => new EmployeeActor(request.EmployeeId)), request.EmployeeId)
                        : Context.Child(request.EmployeeId);

                    employee.Forward(request);
                    break;
            }
        }
    }
}
namespace Arcadia.Assistant.Organization.Abstractions
{
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.DI.Core;

    public abstract class EmployeeIdsQuery : UntypedActor
    {
        public class RequestAllEmployeeIds
        {
            public static readonly RequestAllEmployeeIds Instance = new RequestAllEmployeeIds();

            public class Response
            {
                public Response(string[] ids)
                {
                    this.Ids = ids;
                }

                public string[] Ids { get; }
            }
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case RequestAllEmployeeIds _:
                    this.GetAllEmployeeIds().PipeTo(this.Sender);
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        protected abstract Task<RequestAllEmployeeIds.Response> GetAllEmployeeIds();

        public static Props Props => Context.DI().Props<EmployeeIdsQuery>();
    }
}
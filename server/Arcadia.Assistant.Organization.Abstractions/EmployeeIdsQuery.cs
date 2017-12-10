namespace Arcadia.Assistant.Organization.Abstractions
{
    using System;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.DI.Core;
    using Akka.Event;

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

            public class Error
            {
                public Exception Exception { get; }

                public Error(Exception exception)
                {
                    this.Exception = exception;
                }
            }
        }

        private readonly ILoggingAdapter logger = Context.GetLogger();

        protected override void PreRestart(Exception reason, object message)
        {
            this.logger.Warning($"Error occurred, {reason.Message}");
            this.Sender.Tell(new RequestAllEmployeeIds.Error(reason));
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
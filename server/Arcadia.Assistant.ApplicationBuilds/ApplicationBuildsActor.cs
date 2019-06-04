namespace Arcadia.Assistant.ApplicationBuilds
{
    using System.Collections.Generic;

    using Akka.Actor;
    using Akka.Persistence;

    public class ApplicationBuildsActor : UntypedPersistentActor, ILogReceive
    {
        private readonly Dictionary<string, int> applicationBuilds = new Dictionary<string, int>();

        public override string PersistenceId { get; } = "application-builds";

        protected override void OnRecover(object message)
        {
            switch (message)
            {
                case ApplicationBuildStored msg:
                    this.applicationBuilds[msg.ApplicationKey] = msg.BuildNumber;
                    break;
            }
        }

        protected override void OnCommand(object message)
        {
            switch (message)
            {
                case GetApplicationBuildNumber msg:
                    GetApplicationBuildNumber.Response response;

                    if (!this.applicationBuilds.TryGetValue(msg.ApplicationKey, out var buildNumber))
                    {
                        response = new GetApplicationBuildNumber.Response(null);
                    }
                    else
                    {
                        response = new GetApplicationBuildNumber.Response(buildNumber);
                    }

                    this.Sender.Tell(response);

                    break;

                case SetApplicationBuildNumber msg:
                    var @event = new ApplicationBuildStored(msg.ApplicationKey, msg.BuildNumber);

                    this.Persist(@event, evt =>
                    {
                        this.applicationBuilds[evt.ApplicationKey] = evt.BuildNumber;
                    });

                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private class ApplicationBuildStored
        {
            public ApplicationBuildStored(string applicationKey, int buildNumber)
            {
                this.ApplicationKey = applicationKey;
                this.BuildNumber = buildNumber;
            }

            public string ApplicationKey { get; }

            public int BuildNumber { get; }
        }
    }
}
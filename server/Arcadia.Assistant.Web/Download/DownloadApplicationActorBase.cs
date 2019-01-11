namespace Arcadia.Assistant.Web.Download
{
    using Akka.Actor;

    public abstract class DownloadApplicationActorBase : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case GetLatestApplicationBuildPath _:
                    this.RespondLatestBuildPath();
                    break;

                case DownloadApplicationBuild _:
                    this.RespondDownloadApplicationBuild();
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        protected abstract void RespondLatestBuildPath();

        protected abstract void RespondDownloadApplicationBuild();
    }
}
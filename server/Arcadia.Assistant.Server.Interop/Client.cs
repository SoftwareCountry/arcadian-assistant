namespace Arcadia.Assistant.Server.Interop
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Akka.Actor;

    public class Client
    {
        private DispatcherPath dispatcherPath;

        private readonly IActorRefFactory actorSystem;

        private ServerActorsCollection serverActors;

        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public Client(DispatcherPath dispatcherPath, IActorRefFactory actorSystem)
        {
            this.dispatcherPath = dispatcherPath;
            this.actorSystem = actorSystem;
        }

        private async Task<ServerActorsCollection> GetServerActors()
        {

            if (this.serverActors != null)
            {
                return this.serverActors;
            }

            await this.semaphore.WaitAsync().ConfigureAwait(false);

            try
            {
                if (this.serverActors != null)
                {
                    return this.serverActors;
                }
                else
                {
                    var dispatcher = this.actorSystem.ActorSelection(this.dispatcherPath.Get());
                    var response = await dispatcher.Ask<Messages.Connect.Response>(Messages.Connect.Instance, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
                    return this.serverActors = response.ServerActors;
                }
            }
            catch (Exception)
            {
                //TODO: log
                throw;
            }
            finally
            {
                this.semaphore.Release();
            }
        }

        public async Task<IActorRef> GetOrganizationActor()
        {
            var serverActors = await this.GetServerActors().ConfigureAwait(false);
            return serverActors.Organization;
        }
    }
}
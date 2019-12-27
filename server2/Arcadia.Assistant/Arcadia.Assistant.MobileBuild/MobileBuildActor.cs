namespace Arcadia.Assistant.MobileBuild
{
    using System.Threading;
    using System.Threading.Tasks;

    using Contracts;

    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Actors;
    using Microsoft.ServiceFabric.Actors.Runtime;

    /// <remarks>
    ///     This class represents an actor.
    ///     Every ActorID maps to an instance of this class.
    ///     The StatePersistence attribute determines persistence and replication of actor state:
    ///     - Persisted: State is written to disk and replicated.
    ///     - Volatile: State is kept in memory only and replicated.
    ///     - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    public class MobileBuildActor : Actor, IMobileBuildActor
    {
        private const string BuildVersionKey = "build_version";
        private const string BuildDataKey = "build_data";
        private readonly ILogger logger;

        /// <summary>
        ///     Initializes a new instance of MobileBuild
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        /// <param name="logger">Logger object interface </param>
        public MobileBuildActor(ActorService actorService, ActorId actorId, ILogger<MobileBuildActor> logger)
            : base(actorService, actorId)
        {
            this.logger = logger;
        }

        public Task<string> GetMobileBuildVersionAsync(CancellationToken cancellationToken)
        {
            this.logger?.LogDebug($"Return version");
            return this.StateManager.GetStateAsync<string>(BuildVersionKey, cancellationToken);
        }

        public Task<byte[]> GetMobileBuildDataAsync(CancellationToken cancellationToken)
        {
            this.logger?.LogDebug($"Return build data");
            return this.StateManager.GetStateAsync<byte[]>(BuildDataKey, cancellationToken);
        }

        public async Task SetMobileBuildData(string version, byte[] data, CancellationToken cancellationToken)
        {
            await this.StateManager.AddOrUpdateStateAsync(BuildVersionKey, version, (key, value) => version, cancellationToken);
            await this.StateManager.AddOrUpdateStateAsync(BuildDataKey, data, (key, value) => data, cancellationToken);
            logger?.LogDebug($"Store build data (length:{data?.Length}) for version {version}");
            await this.StateManager.SaveStateAsync(cancellationToken);
        }

        /// <summary>
        ///     This method is called whenever an actor is activated.
        ///     An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            //ActorEventSource.Current.ActorMessage(this, "Actor activated.");
            this.logger?.LogInformation("Actor activated.");

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see https://aka.ms/servicefabricactorsstateserialization

            this.StateManager.TryAddStateAsync(BuildVersionKey, string.Empty);
            return this.StateManager.TryAddStateAsync(BuildDataKey, new byte[] { });
        }
    }
}
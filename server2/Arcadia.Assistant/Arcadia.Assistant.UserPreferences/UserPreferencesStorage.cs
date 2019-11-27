namespace Arcadia.Assistant.UserPreferences
{
    using Contracts;
    using Microsoft.ServiceFabric.Actors;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using System.Threading;
    using System.Threading.Tasks;

    /// <remarks>
    ///     This class represents an actor.
    ///     Every ActorID maps to an instance of this class.
    ///     The StatePersistence attribute determines persistence and replication of actor state:
    ///     - Persisted: State is written to disk and replicated.
    ///     - Volatile: State is kept in memory only and replicated.
    ///     - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class UserPreferencesStorage : Actor, IUserPreferencesStorage
    {
        private const string StateKey = "preferences";

        /// <summary>
        ///     Initializes a new instance of UserPreferences
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public UserPreferencesStorage(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        public async Task<UserPreferences> Get(CancellationToken cancellationToken)
        {
            var state = await this.StateManager.GetOrAddStateAsync(StateKey, this.DefaultPreferences(), cancellationToken);
            return state;
        }

        public async Task Set(UserPreferences userPreferences, CancellationToken cancellationToken)
        {
            await this.StateManager.SetStateAsync(StateKey, userPreferences, cancellationToken);
            await this.StateManager.SaveStateAsync(cancellationToken);
        }

        /// <summary>
        ///     This method is called whenever an actor is activated.
        ///     An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see https://aka.ms/servicefabricactorsstateserialization

            return this.StateManager.TryAddStateAsync("count", 0);
        }

        private UserPreferences DefaultPreferences()
        {
            return new UserPreferences()
            {
                EmailNotifications = true,
                PushNotifications = true
            };
        }
    }
}
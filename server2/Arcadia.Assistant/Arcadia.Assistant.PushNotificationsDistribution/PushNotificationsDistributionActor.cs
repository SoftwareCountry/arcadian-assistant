namespace Arcadia.Assistant.PushNotificationsDistributor
{
    using System.Threading.Tasks;

    using Contracts;
    using Contracts.Models;

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
    public class PushNotificationsDistributionActor : Actor, IPushNotificationsDistributionActor
    {
        private readonly IPushNotificationDistributor distributor;

        /// <summary>
        ///     Initializes a new instance of PushNotificationsDistributionActor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public PushNotificationsDistributionActor(ActorService actorService, ActorId actorId, IPushNotificationDistributor distributor)
            : base(actorService, actorId)
        {
            this.distributor = distributor;
        }

        /*
        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            //ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see https://aka.ms/servicefabricactorsstateserialization

            return this.StateManager.TryAddStateAsync("count", 0);
        }
        */

        /// <summary>
        ///     Send message through push notifications.
        /// </summary>
        /// <param name="message">Message for send</param>
        /// <returns></returns>
        Task IPushNotificationsDistributionActor.SendPushNotification(PushNotificationMessage message)
        {
            // Requests are not guaranteed to be processed in order nor at most once.
            // The update function here verifies that the incoming count is greater than the current count to preserve order.
            //return this.StateManager.AddOrUpdateStateAsync("count", count, (key, value) => count > value ? count : value, cancellationToken);
            return this.distributor.SendPushNotification(message);
        }
    }
}
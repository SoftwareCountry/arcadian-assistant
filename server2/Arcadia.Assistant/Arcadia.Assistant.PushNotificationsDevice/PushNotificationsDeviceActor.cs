namespace Arcadia.Assistant.PushNotificationsDeviceRegistrator
{
    using System.Collections.Generic;
    using System.Threading;
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
    internal class PushNotificationsDeviceActor : Actor, IPushNotificationsDeviceRegistrationActor
    {
        private const string DeviceTokensKey = "device_tokens";
        private const string DeviceTypesKey = "device_types";

        /// <summary>
        ///     Initializes a new instance of PushNotificationsDeviceActor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public PushNotificationsDeviceActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        public async Task RegisterDevice(RegisterPushNotificationsDevice message, CancellationToken cancellationToken)
        {
            /// TODO: check this functionality
            /*
            if (!PushDeviceTypes.IsKnownType(message.DeviceType))
            {
                return;
            }
            */

            var tokens = await this.StateManager.GetStateAsync<Dictionary<string, HashSet<string>>>(DeviceTokensKey, cancellationToken);
            foreach (var hashSet in tokens.Values)
            {
                if (hashSet.Contains(message.DeviceId))
                {
                    hashSet.Remove(message.DeviceId);
                }
            }

            if (!tokens.TryGetValue(message.EmployeeId, out var deviceTokens))
            {
                deviceTokens = new HashSet<string>();
                tokens.Add(message.EmployeeId, deviceTokens);
            }

            deviceTokens.Add(message.DeviceId);
            var types = await this.StateManager.GetStateAsync<Dictionary<string, string>>(DeviceTypesKey, cancellationToken);
            types[message.DeviceId] = message.DeviceType;

            await this.StateManager.AddOrUpdateStateAsync(DeviceTokensKey, tokens, (key, value) => tokens, cancellationToken);
            await this.StateManager.AddOrUpdateStateAsync(DeviceTypesKey, types, (key, value) => types, cancellationToken);
        }

        public async Task RemoveDevice(RemovePushNotificationsDevice message, CancellationToken cancellationToken)
        {
            var tokens = await this.StateManager.GetStateAsync<Dictionary<string, HashSet<string>>>(DeviceTokensKey, cancellationToken);
            if (!tokens.ContainsKey(message.EmployeeId))
            {
                return;
            }

            if (!tokens.TryGetValue(message.EmployeeId, out var deviceTokens))
            {
                return;
            }

            deviceTokens.Remove(message.DeviceId);
            var types = await this.StateManager.GetStateAsync<Dictionary<string, string>>(DeviceTypesKey, cancellationToken);
            if (types.ContainsKey(message.DeviceId))
            {
                types.Remove(message.DeviceId);
                await this.StateManager.AddOrUpdateStateAsync(DeviceTypesKey, types, (key, value) => types, cancellationToken);
            }

            await this.StateManager.AddOrUpdateStateAsync(DeviceTokensKey, tokens, (key, value) => tokens, cancellationToken);
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

            this.StateManager.TryAddStateAsync(DeviceTokensKey, new Dictionary<string, HashSet<string>>());
            return this.StateManager.TryAddStateAsync(DeviceTypesKey, new Dictionary<string, string>());
        }

        /*
        GetDevicePushTokensByEmployee.Response IPushNotificationDevicesManager.GetDevicePushTokensByApplication(GetDevicePushTokensByEmployee message)
        {
            lock (this.Sync)
            {
                this.deviceTokensByEmployeeId.TryGetValue(message.EmployeeId, out var deviceTokens);

                var devicePushTokens = deviceTokens?.Select(token => new DevicePushToken(token, this.deviceTypeByToken[token]));

                return new GetDevicePushTokensByEmployee.Success(devicePushTokens ?? Enumerable.Empty<DevicePushToken>());
            }
        }

        GetDevicePushTokensByApplication.Response IPushNotificationDevicesManager.GetDeviceTokensByApplication(GetDevicePushTokensByApplication message)
        {
            lock (this.Sync)
            {
                var allDeviceTokens = this.deviceTokensByEmployeeId.Values.SelectMany(x => x);

                var applicationDeviceTokens = allDeviceTokens
                    .Where(token => this.deviceTypeByToken[token] == message.DeviceType)
                    .Select(token => new DevicePushToken(token, message.DeviceType))
                    .ToArray();

                return new GetDevicePushTokensByApplication.Response(applicationDeviceTokens);
            }
        }
        */
    }
}
namespace Arcadia.Assistant.Avatars
{
    using System;
    using System.Security.Cryptography;
    using System.Threading;
    using System.Threading.Tasks;

    using Contracts;

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
    [StatePersistence(StatePersistence.Volatile)]
    internal class Avatar : Actor, IAvatar
    {
        private const int Width = 200;

        private const int Height = 200;

        private const string Mime = "image/jpeg";

        private const string ImageHashKey = "image-hash";
        private const string ImageBytesKey = "image-bytes";

        private readonly ImageResizer resizer = new ImageResizer();

        /// <summary>
        ///     Initializes a new instance of Avatar
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public Avatar(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        /// <summary>
        ///     This method is called whenever an actor is activated.
        ///     An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");
            return Task.CompletedTask;
        }

        public async Task SetSource(byte[] bytes)
        {
            await this.ProcessImage(bytes);
        }

        public async Task<Photo> GetPhoto(CancellationToken cancellationToken)
        {
            var states = await this.StateManager.GetStateNamesAsync(cancellationToken);
            var bytes = await this.StateManager.TryGetStateAsync<byte[]>(ImageBytesKey, cancellationToken);
            if (!bytes.HasValue)
            {
                return null;
            }
            else
            {
                return new Photo
                {
                    Bytes = bytes.Value,
                    Height = Height,
                    MimeType = Mime,
                    Width = Width
                };
            }
        }

        private static string GetBytesHash(byte[] bytes)
        {
            using (var sha512 = SHA512.Create())
            {
                return Convert.ToBase64String(sha512.ComputeHash(bytes)) + Width + Height + Mime;
            }
        }

        private async Task ProcessImage(byte[] bytes)
        {
            if (bytes == null)
            {
                return;
            }

            var lastImageHash = await this.StateManager.TryGetStateAsync<string>(ImageHashKey);

            var newHash = GetBytesHash(bytes);

            if (lastImageHash.HasValue && newHash == lastImageHash.Value)
            {
                return;
            }

            var smallImage = this.resizer.ResizeImage(bytes, Width, Height);

            await this.StateManager.AddOrUpdateStateAsync(ImageBytesKey, smallImage, (key, old) => smallImage);
            await this.StateManager.AddOrUpdateStateAsync(ImageHashKey, newHash, (key, old) => newHash);

            await this.SaveStateAsync();
        }
    }
}
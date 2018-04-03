namespace Arcadia.Assistant.Images
{
    using System;
    using System.IO;
    using System.Runtime.ExceptionServices;
    using System.Security.Cryptography;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    using SixLabors.ImageSharp;

    public class PhotoActor : UntypedActor
    {
        private byte[] imageBytes;

        private const int Width = 200;

        private const int Height = 200;

        private const string Mime = "image/jpeg";

        private string lastImageHash = null;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        [HandleProcessCorruptedStateExceptions]
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case SetSource source:
                    try
                    {
                        this.ProcessImage(source.Bytes);
                    }
                    catch (Exception e)
                    {
                        this.logger.Error(e, "Error occurred while processing image");
                    }

                    break;

                case GetPhoto _:
                    var photo = this.imageBytes != null
                        ? new Photo(Mime, Width, Height, this.imageBytes)
                        : null;

                    this.Sender.Tell(new GetPhoto.Response(photo));
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void ProcessImage(byte[] bytes)
        {
            if (bytes == null)
            {
                return;
            }

            using (var sha512 = SHA512.Create())
            {
                var newHash = Convert.ToBase64String(sha512.ComputeHash(bytes));
                if (newHash == this.lastImageHash)
                {
                    return;
                }

                this.lastImageHash = newHash;
            }

            using (var image = Image.Load(bytes))
            using (var output = new MemoryStream())
            {
                image
                    .Mutate(x => x.Resize(Width, Height));

                image
                    .SaveAsJpeg(output);

                this.imageBytes = output.ToArray();
            }
        }

        public sealed class SetSource
        {
            public SetSource(byte[] bytes)
            {
                this.Bytes = bytes;
            }

            public byte[] Bytes { get; }
        }
    }
}
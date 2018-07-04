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
    using SixLabors.ImageSharp.Processing;
    using SixLabors.ImageSharp.Processing.Transforms;

    public class PhotoActor : UntypedActor
    {
        private byte[] imageBytes;

        private const int Width = 200;

        private const int Height = 200;

        private const string Mime = "image/jpeg";

        private string lastImageHash = null;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        private readonly IActorRef imageResizer;

        public PhotoActor(IActorRef imageResizer)
        {
            this.imageResizer = imageResizer;
        }

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

                case ImageResizer.ResizeImage.ImageResized image:
                    this.imageBytes = image.Bytes;
                    this.lastImageHash = GetBytesHash(this.imageBytes);
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private static string GetBytesHash(byte[] bytes)
        {
            using (var sha512 = SHA512.Create())
            {
                return Convert.ToBase64String(sha512.ComputeHash(bytes));
            }
        }

        private void ProcessImage(byte[] bytes)
        {
            if ((bytes == null) || (GetBytesHash(bytes) == this.lastImageHash))
            {
                return;
            }

            this.imageResizer.Tell(new ImageResizer.ResizeImage(bytes, Width, Height));
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
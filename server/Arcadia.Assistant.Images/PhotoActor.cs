namespace Arcadia.Assistant.Images
{
    using System;
    using System.IO;

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

        private readonly ILoggingAdapter logger = Context.GetLogger();

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
                    this.Sender.Tell(new GetPhoto.Response(new Photo(Mime, Width, Height, Convert.ToBase64String(this.imageBytes))));
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
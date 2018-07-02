namespace Arcadia.Assistant.Images
{
    using System.IO;
    using System.Runtime.ExceptionServices;

    using Akka.Actor;

    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Processing;
    using SixLabors.ImageSharp.Processing.Transforms;

    public class ImageResizer : UntypedActor, ILogReceive
    {
        [HandleProcessCorruptedStateExceptions]
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case ResizeImage cmd:
                    using (var image = Image.Load(cmd.Bytes))
                    using (var output = new MemoryStream())
                    {
                        image
                            .Mutate(x => x.Resize(cmd.Width, cmd.Height));

                        image
                            .SaveAsJpeg(output);

                        this.Sender.Tell(new ResizeImage.ImageResized(output.ToArray()));
                    }
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        public class ResizeImage
        {
            public byte[] Bytes { get; }

            public int Width { get; }

            public int Height { get; }

            public ResizeImage(byte[] bytes, int width, int height)
            {
                this.Bytes = bytes;
                this.Width = width;
                this.Height = height;
            }

            public class ImageResized
            {
                public byte[] Bytes { get; }

                public ImageResized(byte[] bytes)
                {
                    this.Bytes = bytes;
                }
            }
        }
    }
}
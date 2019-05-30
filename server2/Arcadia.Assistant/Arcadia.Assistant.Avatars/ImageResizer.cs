namespace Arcadia.Assistant.Avatars
{
    using System;
    using System.IO;
    using System.Runtime.ExceptionServices;

    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Processing;

    public class ImageResizer
    {
        [HandleProcessCorruptedStateExceptions]
        public byte[] ResizeImage(byte[] source, int width, int height)
        {
            using (var image = Image.Load(source))
            using (var output = new MemoryStream())
            {
                try
                {
                    image.Mutate(x => x.Resize(width, height));
                    image.SaveAsJpeg(output);
                    return output.ToArray();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}
﻿namespace Arcadia.Assistant.Avatars
{
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Processing;
    using System;
    using System.IO;
    using System.Runtime.ExceptionServices;

    public class ImageResizer
    {
        [HandleProcessCorruptedStateExceptions]
        public byte[]? ResizeImage(byte[] source, int width, int height)
        {
            try
            {
                using var image = Image.Load(source);
                using var output = new MemoryStream();
                if (image.Width != image.Height)
                {
                    var squareLength = Math.Min(image.Width, image.Height);
                    image.Mutate(x => x.Crop(squareLength, squareLength));
                }

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
namespace Arcadia.Assistant.Web.Models
{
    using System;

    using Arcadia.Assistant.Organization.Abstractions;

    public class PhotoModel
    {
        public string MimeType { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string Bytes { get; set; }

        public static implicit operator PhotoModel(Photo photo)
        {
            return new PhotoModel()
                {
                    MimeType = photo.MimeType,
                    Bytes = Convert.ToBase64String(photo.Bytes), //TODO: wait for .net 2.1 to change on System.Buffers.Text
                    Height = photo.Height,
                    Width = photo.Width
                };
        }
    }
}
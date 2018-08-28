namespace Arcadia.Assistant.Web.Models
{
    using System;
    using System.Runtime.Serialization;

    using Arcadia.Assistant.Organization.Abstractions;

    [DataContract]
    public class PhotoModel
    {
        [DataMember]
        public string MimeType { get; set; }

        [DataMember]
        public int Width { get; set; }

        [DataMember]
        public int Height { get; set; }

        [DataMember]
        public string Base64 { get; set; }

        public static implicit operator PhotoModel(Photo photo)
        {
            if (photo?.Bytes == null)
            {
                return null;
            }

            return new PhotoModel()
                {
                    MimeType = photo.MimeType,
                    Base64 = Convert.ToBase64String(photo.Bytes), //TODO: wait for .net 2.1 to change on System.Buffers.Text
                    Height = photo.Height,
                    Width = photo.Width
                };
        }
    }
}
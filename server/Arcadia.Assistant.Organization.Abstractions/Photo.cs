namespace Arcadia.Assistant.Organization.Abstractions
{
    public class Photo
    {
        public string MimeType { get; }

        public int Width { get; }

        public int Height { get; }

        public string Base64 { get; }

        public Photo(string mimeType, int width, int height, string base64)
        {
            this.MimeType = mimeType;
            this.Width = width;
            this.Height = height;
            this.Base64 = base64;
        }
    }
}
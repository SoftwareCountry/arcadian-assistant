namespace Arcadia.Assistant.Organization.Abstractions
{
    public class Photo
    {
        public string MimeType { get; }

        public int Width { get; }

        public int Height { get; }

        public byte[] Bytes { get; } //TODO: once core 2.1 released, switch to ReadOnlySpan

        public Photo(string mimeType, int width, int height, byte[] bytes)
        {
            this.MimeType = mimeType;
            this.Width = width;
            this.Height = height;
            this.Bytes = bytes;
        }
    }
}
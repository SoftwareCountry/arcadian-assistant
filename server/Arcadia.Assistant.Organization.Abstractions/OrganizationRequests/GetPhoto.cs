namespace Arcadia.Assistant.Organization.Abstractions.OrganizationRequests
{
    public sealed class GetPhoto
    {
        private GetPhoto()
        {
        }

        public static readonly GetPhoto Instance = new GetPhoto();

        public sealed class Response
        {
            public Photo Photo { get; }

            public Response(Photo photo)
            {
                this.Photo = photo;
            }
        }
    }
}
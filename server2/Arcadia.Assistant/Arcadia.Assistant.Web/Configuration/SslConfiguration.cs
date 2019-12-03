namespace Arcadia.Assistant.Web.Configuration
{
    public class SslConfiguration : ISslSettings
    {
        public bool SslOffloading { get; set; } = false;
    }
}
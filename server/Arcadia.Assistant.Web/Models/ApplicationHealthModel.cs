namespace Arcadia.Assistant.Web.Models
{
    public class ApplicationHealthModel
    {
        public bool IsServerAlive { get; set; }

        public bool Is1CAlive { get; set; }

        public bool IsDatabaseAlive { get; set; }
    }
}
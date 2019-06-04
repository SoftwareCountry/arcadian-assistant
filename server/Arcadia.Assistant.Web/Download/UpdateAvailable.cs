namespace Arcadia.Assistant.Web.Download
{
    public class UpdateAvailable
    {
        public UpdateAvailable(ApplicationTypeEnum applicationType)
        {
            this.ApplicationType = applicationType;
        }

        public ApplicationTypeEnum ApplicationType { get; }
    }
}
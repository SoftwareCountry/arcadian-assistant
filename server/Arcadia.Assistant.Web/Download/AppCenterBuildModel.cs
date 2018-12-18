namespace Arcadia.Assistant.Web.Download
{
    using System;

    public class AppCenterBuildModel
    {
        public int Id { get; set; }

        public DateTime FinishTime { get; set; }

        public string Status { get; set; }

        public string Result { get; set; }
    }
}
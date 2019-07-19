namespace Arcadia.Assistant.Organization.Abstractions.OrganizationRequests
{
    using System.Collections.Generic;

    public class GetDepartmentFeatures
    {
        public GetDepartmentFeatures(string departmentId)
        {
            this.DepartmentId = departmentId;
        }

        public string DepartmentId { get; }

        public abstract class Response
        {
        }

        public class Success : Response
        {
            public Success(IEnumerable<string> features)
            {
                this.Features = features;
            }

            public IEnumerable<string> Features { get; }
        }

        public class NotFound : Response
        {
            public static NotFound Instance = new NotFound();
        }
    }
}
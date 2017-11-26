namespace Arcadia.Assistant.Organization.Abstractions
{
    public static class OrganizationRequests
    {
        /// <summary>
        /// Should return an instance of <seealso cref="EmployeeInfo"/>
        /// </summary>
        public sealed class RequestEmployeeInfo
        {
            public string EmployeeId { get; }

            public RequestEmployeeInfo(string employeeId)
            {
                this.EmployeeId = employeeId;
            }

            public sealed class Success
            {
                public Success(EmployeeInfo employeeInfo)
                {
                    this.EmployeeInfo = employeeInfo;
                }

                public EmployeeInfo EmployeeInfo { get; }
            }

            public sealed class EmployeeNotFound
            {
                public EmployeeNotFound(string employeeId)
                {
                    this.EmployeeId = employeeId;
                }

                public string EmployeeId { get; }
            }
        }
    }
}
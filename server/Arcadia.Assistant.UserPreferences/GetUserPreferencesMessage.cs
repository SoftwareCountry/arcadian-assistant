namespace Arcadia.Assistant.UserPreferences
{
    public class GetUserPreferencesMessage
    {
        public GetUserPreferencesMessage(string employeeId)
        {
            this.EmployeeId = employeeId;
        }

        public string EmployeeId { get; }

        public class Response
        {
            public Response(UserPreferences userPreferences)
            {
                UserPreferences = userPreferences;
            }

            public UserPreferences UserPreferences { get; }
        }
    }
}
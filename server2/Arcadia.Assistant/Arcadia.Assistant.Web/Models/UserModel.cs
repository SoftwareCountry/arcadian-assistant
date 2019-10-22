namespace Arcadia.Assistant.Web.Models
{
    using System.Runtime.Serialization;

    [DataContract]
    public class UserModel
    {
        public UserModel(string username, string employeeId)
        {
            this.Username = username;
            this.EmployeeId = employeeId;
        }

        [DataMember]
        public string Username { get; }

        [DataMember]
        public string EmployeeId { get; }
    }
}
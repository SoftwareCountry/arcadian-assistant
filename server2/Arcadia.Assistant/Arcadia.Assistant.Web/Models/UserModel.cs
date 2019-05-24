namespace Arcadia.Assistant.Web.Models
{
    using System.Runtime.Serialization;

    [DataContract]
    public class UserModel
    {
        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string EmployeeId { get; set; }
    }
}
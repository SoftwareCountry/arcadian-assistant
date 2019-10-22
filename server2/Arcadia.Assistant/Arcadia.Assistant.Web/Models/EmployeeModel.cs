namespace Arcadia.Assistant.Web.Models
{
    using System;
    using System.Runtime.Serialization;

    using Employees.Contracts;

    [DataContract]
    public class EmployeeModel
    {
        public EmployeeModel(string employeeId, string email)
        {
            this.EmployeeId = employeeId;
            this.Email = email;
        }

        [DataMember]
        public string EmployeeId { get; }

        [DataMember]
        public string? FirstName { get; set; }

        [DataMember]
        public string? LastName { get; set; }

        [DataMember]
        public string Name => $"{this.LastName} {this.FirstName}".Trim();

        [DataMember]
        public string Email { get; }

        [DataMember]
        public Sex Sex { get; set; }

        [DataMember]
        public string? PhotoUrl { get; set; }

        [DataMember]
        public string? Position { get; set; }

        [DataMember]
        public string? DepartmentId { get; set; }

        [DataMember]
        public string? MobilePhone { get; set; }

        [DataMember]
        public DateTime? BirthDate { get; set; }

        [DataMember]
        public DateTime? HireDate { get; set; }

        [DataMember]
        public string? RoomNumber { get; set; }

        [DataMember]
        public int? VacationDaysLeft { get; set; }

        /// <summary>
        /// Positive values means that these days must be worked out.
        /// Negative means that these can be taken as days off
        /// </summary>
        [DataMember]
        public int? HoursCredit { get; set; }

        public static EmployeeModel FromMetadata(EmployeeMetadata metadata)
        {
            return new EmployeeModel(metadata.EmployeeId.ToString(), metadata.Email)
                {
                    BirthDate = metadata.BirthDate,
                    DepartmentId = metadata.DepartmentId?.ToString(),
                    HireDate = metadata.HireDate,
                    MobilePhone = metadata.MobilePhone,
                    FirstName = metadata.FirstName,
                    LastName = metadata.LastName,
                    Position = metadata.Position,
                    Sex = metadata.Sex,
                    RoomNumber = metadata.RoomNumber
                };
        }
    }
}
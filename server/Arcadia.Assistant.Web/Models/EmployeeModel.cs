namespace Arcadia.Assistant.Web.Models
{
    using System;
    using System.Runtime.Serialization;

    using Arcadia.Assistant.Organization.Abstractions;

    [DataContract]
    public class EmployeeModel
    {
        [DataMember]
        public string EmployeeId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public Sex Sex { get; set; }

        [DataMember]
        public string PhotoUrl { get; set; }

        [DataMember]
        public string Position { get; set; }

        [DataMember]
        public string DepartmentId { get; set; }

        [DataMember]
        public string MobilePhone { get; set; }

        [DataMember]
        public DateTime? BirthDate { get; set; }

        [DataMember]
        public DateTime? HireDate { get; set; }

        [DataMember]
        public string RoomNumber { get; set; }

        [DataMember]
        public int? VacationDaysLeft { get; set; }

        /// <summary>
        /// Positive values means that these days must be worked out.
        /// Negative means that these can be taked as days off
        /// </summary>
        [DataMember]
        public int? HoursCredit { get; set; }

        public static EmployeeModel FromMetadata(EmployeeMetadata metadata)
        {
            return new EmployeeModel()
                {
                    EmployeeId = metadata.EmployeeId,
                    BirthDate = metadata.BirthDate,
                    DepartmentId = metadata.DepartmentId,
                    Email = metadata.Email,
                    HireDate = metadata.HireDate,
                    MobilePhone = metadata.MobilePhone,
                    Name = metadata.Name,
                    Position = metadata.Position,
                    Sex = metadata.Sex,
                    RoomNumber = metadata.RoomNumber
                };
        }
    }
}
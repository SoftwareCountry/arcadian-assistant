namespace Arcadia.Assistant.Web.Models
{
    using Organization.Contracts;
    using System.Runtime.Serialization;

    [DataContract]
    public class DepartmentModel
    {
        public DepartmentModel(string departmentId, string abbreviation, string name)
        {
            this.DepartmentId = departmentId;
            this.Abbreviation = abbreviation;
            this.Name = name;
        }

        [DataMember]
        public string DepartmentId { get; }

        [DataMember]
        public string Abbreviation { get; }

        [DataMember]
        public string Name { get; }

        [DataMember]
        public string? ParentDepartmentId { get; set; }

        [DataMember]
        public string? ChiefId { get; set; }

        [DataMember]
        public int PeopleCount { get; set; } = 0;

        public static DepartmentModel FromMetadata(DepartmentMetadata department)
        {
            return new DepartmentModel(department.DepartmentId.ToString(), department.Abbreviation, department.Name)
            {
                ChiefId = department.ChiefId?.Value.ToString(),
                ParentDepartmentId = department.ParentDepartmentId?.ToString(),
                PeopleCount = department.PeopleCount
            };
        }
    }
}
namespace Arcadia.Assistant.Web.Models
{
    using System.Runtime.Serialization;

    using Organization.Contracts;

    [DataContract]
    public class DepartmentModel
    {
        [DataMember]
        public string DepartmentId { get; set; }

        [DataMember]
        public string Abbreviation { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string ParentDepartmentId { get; set; }

        [DataMember]
        public string ChiefId { get; set; }

        [DataMember]
        public int PeopleCount { get; set; }

        public static DepartmentModel FromMetadata(DepartmentMetadata department)
        {
            return new DepartmentModel()
            {
                Abbreviation = department.Abbreviation,
                ChiefId = department.ChiefId?.Value.ToString(),
                DepartmentId = department.DepartmentId,
                Name = department.Name,
                ParentDepartmentId = department.ParentDepartmentId,
                PeopleCount = department.PeopleCount
            };
        }
    }
}
namespace Arcadia.Assistant.Configuration.Configuration
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    [DataContract]
    public class DepartmentFeaturesMapping
    {
        [DataMember]
        [Required]
        public string DepartmentId { get; set; }

        [DataMember]
        [Required]
        public IEnumerable<string> Features { get; set; }
    }
}
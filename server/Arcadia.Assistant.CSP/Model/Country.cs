using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class Country
    {
        public Country()
        {
            Visa = new HashSet<Visa>();
            VisaHistory = new HashSet<VisaHistory>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string NameRus { get; set; }
        public string Code2 { get; set; }
        public string Code3 { get; set; }
        public bool IsSchengen { get; set; }

        public ICollection<Visa> Visa { get; set; }
        public ICollection<VisaHistory> VisaHistory { get; set; }
    }
}

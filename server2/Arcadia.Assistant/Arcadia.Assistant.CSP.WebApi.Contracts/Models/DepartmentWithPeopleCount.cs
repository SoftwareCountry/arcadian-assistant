using System;
using System.Collections.Generic;
using System.Text;

namespace Arcadia.Assistant.CSP.WebApi.Contracts.Models
{
    public class DepartmentWithPeopleCount
    {
        public Department Department { get; set; } = new Department();

        public int? ActualChiefId { get; set; }

        public int PeopleCount { get; set; }
    }
}

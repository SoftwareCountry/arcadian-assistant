using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class Cspalert
    {
        public int AlertTypeId { get; set; }
        public DateTime AlertDate { get; set; }
        public string AlertText { get; set; }
        public int Id { get; set; }

        public CspalertType AlertType { get; set; }
    }
}

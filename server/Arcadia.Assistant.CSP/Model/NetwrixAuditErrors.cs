using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class NetwrixAuditErrors
    {
        public Guid ErrorId { get; set; }
        public DateTime ErrorTime { get; set; }
        public string Workstation { get; set; }
        public string Application { get; set; }
        public string DataBaseName { get; set; }
        public string TableName { get; set; }
        public int MessageId { get; set; }
        public string Message { get; set; }
    }
}

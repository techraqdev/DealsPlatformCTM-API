using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class AuditLogsDTO
    {
        public Guid Uid { get; set; }
        public string? ProjectCode { get; set; }
        public string? ProjectName { get; set; }
        public Guid? Projectid { get; set; }
        public string? OldRowData { get; set; }
        public string? NewRowData { get; set; }
        public DateTime? DmlTimestamp { get; set; }
        public string? DmlCreatedBy { get; set; }
        public string? CreatedBy { get; set; }
        public string? DmlType { get; set; }
        public string? ProjectStatus { get; set; }
        public string? Sbu { get; set; }
        public string? LegalEntity { get; set; }
        public string? SrcTableName { get; set; }
        public bool? IsModified { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class AuditSearchDTO
    {
        public PageQueryModelDTO PageQueryModel = new();
        public string? ProjectCode { get; set; }
        public Guid? UserId { get; set; }
        public bool IsAdmin { get; set; }
        public string? ProjectName { get; set; }
        public string? UserName { get; set; }
        public object? LoginUserEmail { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? SrcTable { get; set; }
        public bool ExportToExcel { get; set; }
    }
}

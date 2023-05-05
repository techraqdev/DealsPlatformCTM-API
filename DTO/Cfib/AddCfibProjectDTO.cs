using DTO.Ctm;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Cfib
{
    public class AddCfibProjectDTO
    {
        public Guid ProjectId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int SubsectorId { get; set; }
        public Guid UserId { get; set; }

        [StringLength(5000)]
        public string UniqueIdentifier { get; set; }
        public bool isNewAdd { get; set; }
        public int Sector { get; set; }
    }
}

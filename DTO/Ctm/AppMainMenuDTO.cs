using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Ctm
{
    public class AppMainMenuDTO
    {
        public AppMainMenuDTO()
        {
            Children = new List<AppMainMenuDTO>();
        }
        public int AppMenuId { get; set; }
        public string Title { get; set; }
        public string HRef { get; set; }
        public string Icon { get; set; }
        public int? OrderId { get; set; }
        public bool Collapse { get; set; }
        public List<AppMainMenuDTO> Children { get; set; }

    }
}

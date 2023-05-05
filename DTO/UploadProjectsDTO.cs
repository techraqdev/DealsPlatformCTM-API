using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class UploadProjectsDTO
    {
        public string ProjectErrorMsg { get; set;}
        public string ProjectsHeaderErrorMsg { get; set;}
        public bool ProjectsError { get; set;}
        public bool UploadSucess { get; set;}
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO;
public class ClientResponseDTO
{    
    public string ProjectCode { get; set; }
    public string ProjectCredApproved { get; set; }
    public string NotInCredClientApprovalPending { get; set; }
    public bool IsHeaderInvalid { get; set; }
    public string InValidData { get; set; }
    public string InValidDateProCodes { get; set; }
    public string InValidStatusProCodes { get; set; }
    public string InValidEngamentProCodes { get; set; }

}

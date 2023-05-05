using System.ComponentModel.DataAnnotations;

namespace DTO.Ctm
{
    public class LoginDTO
    {
        [StringLength(5000)]
        public string? DomainName { get; set; }
        [StringLength(5000)]
        public string? UserName { get; set; }
        [StringLength(5000)]
        public string? Password { get; set; }
    }
}

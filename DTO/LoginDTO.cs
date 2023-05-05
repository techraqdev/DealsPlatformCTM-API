using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class LoginDTO
    {
        [StringLength(5000)]
        public string UserName { get; set; } = default!;
        [StringLength(5000)]
        public string Password { get; set; } = default!;
    }
}

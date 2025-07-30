using Microsoft.AspNetCore.Identity;

namespace Emba_IP.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string SurName { get; set; }
        public bool? IsSuperAdmin { get; set; }
        public bool? IsAdmin { get; set; }
    }
}

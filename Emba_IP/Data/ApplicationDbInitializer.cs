using Emba_IP.Models;
using Microsoft.AspNetCore.Identity;
using static System.Formats.Asn1.AsnWriter;

namespace Emba_IP.Data
{
    public class ApplicationDbInitializer
    {
        private const string DefaultRole = "SuperAdmin";
        private const string DefaultUserName = "Administrator";
        private const string DefaultEmail = "superAdmin@emba.com";
        private const string DefaultPassword = "Emba_123456789";


        public static async Task SeedDefaultUserAndRoleAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            IdentityResult roleCreated;
            if (await roleManager.FindByNameAsync(DefaultRole) == null)
            {
                var defaultRole = new IdentityRole
                {
                    Name = DefaultRole
                };
                roleCreated = await roleManager.CreateAsync(defaultRole);
            }
            else
            {
                roleCreated = IdentityResult.Success;
            }
        }

    }
}

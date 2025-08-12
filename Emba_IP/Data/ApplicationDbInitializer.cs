using Emba_IP.Models;
using Microsoft.AspNetCore.Identity;
using static System.Formats.Asn1.AsnWriter;

namespace Emba_IP.Data
{
    public class ApplicationDbInitializer
    {

        public static async Task SeedDefaultUserAndRoleAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SuperAdminSettings adminSettings)
        {
            IdentityResult roleCreated;
            if (await roleManager.FindByNameAsync(adminSettings.Role) == null)
            {
                await roleManager.CreateAsync(new IdentityRole { Name = adminSettings.Role });
            }

            var user = await userManager.FindByEmailAsync(adminSettings.Email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    Name = "Super",
                    SurName = "Admin",
                    UserName = adminSettings.UserName,
                    Email = adminSettings.Email,
                    EmailConfirmed = true,
                    IsSuperAdmin=true
                };

                await userManager.CreateAsync(user, adminSettings.Password);

                // 3. İstifadəçiyə rolu təyin et
                await userManager.AddToRoleAsync(user, adminSettings.Role);
            }
        }

    }
}

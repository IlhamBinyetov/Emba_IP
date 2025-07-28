using Microsoft.AspNetCore.Identity;

namespace Emba_IP.Extensions
{
    public static class IdentityConfigurationExtensions
    {
        public static IServiceCollection ConfigureIdentityPolicies(this IServiceCollection services)
        {
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
            });
            return services;
        }
    }
}

using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Emba_IP.Extensions
{
    public static class LdapConfigurationExtensions
    {
        public static IServiceCollection ConfigureLdapAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
                .AddNegotiate();

            services.AddAuthorization();

            return services;
        }
    }
}

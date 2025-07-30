using Emba_IP.Models;
using Microsoft.Extensions.Options;

namespace Emba_IP.Services
{
    using Novell.Directory.Ldap;
    using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

    public class LdapService
    {
        private readonly LdapSettings _ldapSettings;

        public LdapService(IOptions<LdapSettings> ldapOptions)
        {
            _ldapSettings = ldapOptions.Value;
        }

        public async  Task<bool> Authenticate(string username, string password)
        {
            try
            {
                using var ldapConnection = new LdapConnection();
                await ldapConnection.ConnectAsync(_ldapSettings.Path, _ldapSettings.Port);
                await ldapConnection.BindAsync(_ldapSettings.ServiceUsername, _ldapSettings.ServicePassword);
                 

                var searchFilter = $"(sAMAccountName={username})";

                var result = await ldapConnection.SearchAsync(
                    _ldapSettings.SearchBase,
                    LdapConnection.ScopeSub,
                    searchFilter,
                    null,
                    false
                );

                if (result.HasMoreAsync().Result)
                {
                    var userEntry = await result.NextAsync();
                    var userDn = userEntry.Dn;

                    
                    using var userConn = new LdapConnection();
                    await userConn.ConnectAsync(_ldapSettings.Path, _ldapSettings.Port);
                    await userConn.BindAsync(userDn, password);

                    return userConn.Bound;
                }

                return false;
            }
            catch (LdapException ex)
            {
                Console.WriteLine($"LDAP ERROR: {ex.Message}");
                return false;
            }
        }
    }

}

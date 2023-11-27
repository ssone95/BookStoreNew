using IdentityServer4.Stores;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace IdentityServerOld.Helpers
{
    public static class HelperMethods
    {
        public static async Task<bool> IsPkceClientAsync(this IClientStore store, string client_id)
        {
            if (!string.IsNullOrWhiteSpace(client_id))
            {
                var client = await store.FindEnabledClientByIdAsync(client_id);
                return client?.RequirePkce == true;
            }

            return false;
        }
    }
}

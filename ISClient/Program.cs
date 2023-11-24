using IdentityModel.Client;

namespace ISClient
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            using (var httpClient = new HttpClient())
            {
                var disco = await httpClient.GetDiscoveryDocumentAsync("http://localhost:5000");

                if (disco.IsError)
                {
                    Console.WriteLine($"{disco.Error} : {disco.HttpErrorReason}  : {disco.ErrorType}");
                    return;
                }

                var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest()
                {
                    Address = disco.TokenEndpoint,

                    ClientId = "bookStoreClient",
                    ClientSecret = "Secret123$",

                    Scope = "bookStoreApiPostman"
                });

                if (tokenResponse.IsError)
                {
                    Console.WriteLine($"{tokenResponse.Error} : {tokenResponse.ErrorDescription} : {tokenResponse.ErrorType}");
                    return;
                }

                Console.WriteLine($"tokenResponse: {tokenResponse.Json}");
            }
        }
    }
}
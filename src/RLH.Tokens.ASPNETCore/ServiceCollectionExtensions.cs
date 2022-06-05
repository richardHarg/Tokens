using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace RLH.Tokens.ASPNETCore
{
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// Adds the required scoped ITokenService to the serviceCollection
        /// </summary>
        /// <param name="serviceCollection">ServiceCollection to populate</param>
        /// <param name="configuration">Configuration to locate 'TokenConfig' values</param>
        /// <returns>ServiceCollection to chain methods</returns>
        public static ServiceCollection AddTokenService(this ServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddScoped<ITokenService, OptionsTokenService>();
            serviceCollection.Configure<TokenConfig>(configuration.GetSection("TokenConfig"));

            return serviceCollection;
        }

        /// <summary>
        /// Adds the required scoped ITokenService to the serviceCollection
        /// </summary>
        /// <param name="serviceCollection">ServiceCollection to populate</param>
        /// <param name="tokenConfig">'TokenConfig' values</param>
        /// <returns>ServiceCollection to chain methods</returns>
        public static ServiceCollection AddTokenService(this ServiceCollection serviceCollection,Action<TokenConfig> tokenConfig)
        {
            serviceCollection.AddScoped<ITokenService, OptionsTokenService>();
            serviceCollection.Configure(tokenConfig);

            return serviceCollection;
        }
    }
}

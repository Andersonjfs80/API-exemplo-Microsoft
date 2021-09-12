using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ContosoPizza.Models;

namespace ContosoPizza.Middlewares
{
    public static class JwtMiddleware
    {
        public static void AddJwtMiddleware(this IServiceCollection services, IConfiguration configuration)
        {
            UseJwtMiddleware(services, configuration);
        }

        private static void UseJwtMiddleware(IServiceCollection services, IConfiguration configuration)
        {
            var tokenConfiguration = 
              configuration.GetSection("JwtTokenConfiguration").Get<JwtTokenConfiguration>();

            services.AddSingleton<JwtTokenConfiguration>(tokenConfiguration);
        }
        
    }
}
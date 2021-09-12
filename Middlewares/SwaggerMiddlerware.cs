using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using Microsoft.OpenApi.Models;

namespace ContosoPizza.Middlewares
{
    public static class SwaggerMiddlerware
    {
        public static void AddSwaggerService(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "jwtAndersonAuthApi",
                        Version = "v1",
                        Description = "Auhorization Jwt Api",
                        Contact = new OpenApiContact
                        {
                            Name = "AndersonJFS80",
                            Url = new Uri("https://github.com/andersonjfs80/jwtAndersonAuthApi")
                        }
                    });

                var caminhoAplicacao = AppContext.BaseDirectory;
                var caminhoXmlDoc =
                     Path.Combine(caminhoAplicacao, $"jwtAndersonAuthApi.Application.xml");

                c.IncludeXmlComments(caminhoXmlDoc);
            });
        }

        public static void AddSwaggerApp(this IApplicationBuilder app, string routePrefix)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json",
                    "Anderson Auhorization Jwt Api");

                c.RoutePrefix = routePrefix;
            });
        }
    }
}
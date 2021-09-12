using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using ContosoPizza.Middlewares;

namespace ContosoPizza
{
//https://docs.microsoft.com/pt-br/learn/modules/build-web-api-aspnet-core/3-exercise-create-web-api    
//Código para cria o projeto apos criar a pasta do projeto
//dotnet new webapi --no-https
//dotnet build compilar aplicação
//dotnet run Hospeda a API Web com o servidor Web Kestrel do ASP.NET Core
//Executar no browser para testar http://localhost:5000/weatherforecast
//dotnet tool install -g Microsoft.dotnet-httprepl
//O comando anterior instala a ferramenta de linha de comando 
//REPL (Read-Eval-Print Loop) HTTP do .NET que será usada para fazer solicitações HTTP para a API Web.
//httprepl http://localhost:5000 Outra opção é executar o seguinte comando a qualquer momento enquanto HttpRepl estiver em execução:
//(Disconnected)> connect http://localhost:5000
//Explore os pontos de extremidade disponíveis executando o seguinte comando: ls
//Navegue até o ponto de extremidade WeatherForecast executando o seguinte comando: cd WeatherForecast
//O seguinte comando retorna as APIs disponíveis para o ponto de extremidade WeatherForecast:
//CLI do .NET
//http://localhost:5000/> cd WeatherForecast
//WeatherForecast    [GET]
//Faça uma solicitação GET no HttpRepl usando o seguinte comando:
//CLI do .NET
//get   
//Saia da sessão de HttpRepl atual usando o seguinte comando: exit
//mkdir Models criando a pasta models
//Criando o arquivo Pizza.cs
//mkdir Services criando a pasta service
//Criando a classe de serviço da aplicação PizzaService.cs
//Conecte-se à API Web executando o seguinte comando:
//CLI do .NET
//httprepl http://localhost:5000
//Alternatively, run the following command at any time while the HttpRepl is running:
//For example: ```dotnetcli (Disconnected)> connect http://localhost:5000
//Para ver o ponto de extremidade de pizza recém-disponível, execute o seguinte comando:
//CLI do .NET: ls
//Digite o controller para testar a API
//cd Pizza
//O comando anterior retorna as APIs disponíveis para o ponto de extremidade Pizza:
//CLI do .NET
//http://localhost:5000/> cd Pizza
// /Pizza    [GET]
//Faça uma solicitação GET no HttpRepl usando o seguinte comando:
//CLI do .NET
//get
//Para consultar apenas uma pizza, podemos fazer outra solicitação GET, mas passar um parâmetro id usando o seguinte comando:
//CLI do .NET
//get 1
//get 5 quando não exite o item
//Fazer um post para testar post -c "{"name":"Hawaii", "isGlutenFree":false}"
//Fazer um put para testar put 3 -c  "{"id": 3, "name":"Hawaiian", "isGlutenFree":false}"
//Fazer um delete para testar delete 3
//Acessar o Swagger http://localhost:5000/Swagger

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddCors();
            services.AddControllers();
            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "ContosoPizza", Version = "v1" });
                opt.AddSecurityDefinition("bearer", new OpenApiSecurityScheme 
                {                        
                    Description = "Autenticação baseada em Json Web Token (JWT)",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = "bearer",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT"
                });                

                //https://docs.microsoft.com/pt-br/learn/modules/improve-api-developer-experience-with-swagger/
                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {                        
                    {                            
                        new OpenApiSecurityScheme                            
                        {
                            Reference = new OpenApiReference 
                            {                                    
                                Type = ReferenceType.SecurityScheme,
                                Id = "bearer" 
                            },
                            Type = SecuritySchemeType.Http,           
                            Scheme = "bearer",
                            BearerFormat = "JWT"
                        }, 
                        new List<string>() 
                    }
                });
            });

            services.AddSingleton<IConfiguration>(Configuration);

            IdentityModelEventSource.ShowPII = true;

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
               .AddJwtBearer(options =>
                  {
                      options.RequireHttpsMetadata = false;
                      options.SaveToken = true;
                      options.TokenValidationParameters = new TokenValidationParameters
                     {
                       ValidateIssuer = false,
                       ValidateAudience = false,
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,                       
                       ValidIssuer = Configuration["JwtTokenConfiguration:Issuer"],
                       ValidAudience = Configuration["JwtTokenConfiguration:Audience"],
                         IssuerSigningKey = new SymmetricSecurityKey(
                             Encoding.UTF8.GetBytes(Configuration["JwtTokenConfiguration:Key"]))
                     };
                 });            

        }

        private static void LoadCustomMiddlers(IServiceCollection services, IConfiguration configuration)
        {
            services.AddJwtMiddleware(configuration);
            //services.AddLoggerMiddleware();
            //services.AddDependencyInjection();
            services.AddSwaggerService();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ContosoPizza v1"));

            app.UseRouting();

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());            

            app.UseAuthentication();
            app.UseAuthorization();

            //A ordem é importante dessa forma não funciona
            //Na tag do controller [Autorize] tem identificar o [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
            //controlador ignora auth cookie e se concentra apenas em jwt
            //app.UseAuthorization(); 
            //app.UseAuthentication();

            app.AddSwaggerApp("docs");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

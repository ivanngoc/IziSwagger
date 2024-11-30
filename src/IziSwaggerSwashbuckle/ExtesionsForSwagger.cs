using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace IziSwaggerSwashbuckle
{
    public static class ExtesionsForSwagger
    {
        public static IServiceCollection AddSwaggerGenWithSpecificOfIziHardGames(this IServiceCollection services)
        {

            services.AddSwaggerGen(options =>
            {
                //options.OperationFilter<StatusCode400Annotations>();
                //options.OperationFilter<AddRequiredHeaderParameter>();
                //options.SchemaFilter<SwaggerTryItOutDefaultValueFilter>();
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ToDo API",
                    Description = "An ASP.NET Core Web API for managing ToDo items",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Example Contact",
                        Url = new Uri("https://example.com/contact")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Example License",
                        Url = new Uri("https://example.com/license")
                    }
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });

                var asm = Assembly.GetEntryAssembly();
                ArgumentNullException.ThrowIfNull(asm);
                var xmlFilename = $"{asm.GetName().Name}.xml";
                // enable Swashbuckle.AspNetCore.Annotations
                options.EnableAnnotations();
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            return services;
        }
    }
}

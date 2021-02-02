using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace SeatsSuggestions.Api
{
    /// <summary>
    /// Extension methods for <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/> instances.
    /// </summary>
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddSwaggerGeneration(this IServiceCollection services, OpenApiContact apiContact, string swaggerTitle, Type callerType)
        {
            return services.AddSwaggerGen(options =>
            {
                // Resolve the temporary IApiVersionDescriptionProvider service  
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

                // Add a swagger document for each discovered API version  
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName, new OpenApiInfo
                    {
                        Title = swaggerTitle + $" {description.ApiVersion}",
                        Version = description.ApiVersion.ToString(),
                        Contact = apiContact
                    });
                }

                // Add a custom filter for setting the default values  
                options.OperationFilter<SwaggerDefaultValues>();

                // Tells swagger to pick up the output XML document file  
                options.IncludeXmlComments(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"{callerType.Assembly.GetName().Name}.xml"));
            });
        }

        public static IServiceCollection AddVersioning(this IServiceCollection services)
        {
            return services.AddVersionedApiExplorer(options =>
            {
                //The format of the version added to the route URL  
                options.GroupNameFormat = "'v'VVV";

                //Tells swagger to replace the version in the controller route  
                options.SubstituteApiVersionInUrl = true;
            }).AddApiVersioning(
                options =>
                {
                    options.ReportApiVersions = true;
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.DefaultApiVersion = new ApiVersion(new DateTime(2016, 7, 1));
                });
        }
    }
}
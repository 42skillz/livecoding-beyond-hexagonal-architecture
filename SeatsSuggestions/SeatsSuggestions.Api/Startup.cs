using System;
using System.IO;
using System.Reflection;
using ExternalDependencies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using SeatsSuggestions.Domain;
using SeatsSuggestions.Domain.Ports;
using SeatsSuggestions.Infra;
using SeatsSuggestions.Infra.Adapter;
using SeatsSuggestions.Infra.Helpers;

namespace SeatsSuggestions.Api
{
    public class Startup
    {
        private const string ApiContactName = "42 skillz";
        private const string ApiContactEmail = "contact@42skillz.com";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddMvcOptions(o => o.EnableEndpointRouting = false);

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                });

            services.AddVersioning();

            ConfigurePortsAndAdapters(services);

            var openApiContact = new OpenApiContact
            {
                Name = ApiContactName,
                Email = ApiContactEmail
            };

            var swaggerTitle = $"{GetType().Assembly.GetCustomAttribute<AssemblyProductAttribute>().Product}";

            services.AddSwaggerGen(o =>
                o.IncludeXmlComments(
                    $"{Path.Combine(AppContext.BaseDirectory, Assembly.GetExecutingAssembly().GetName().Name)}.xml"));

            services.AddSwaggerGeneration(openApiContact, swaggerTitle, GetType());
        }

        private static void ConfigurePortsAndAdapters(IServiceCollection services)
        {
            // The 3 steps initialization of the Hexagonal Architecture  -------------------------------------------------------------------
            
            // Step1: Instantiate the "I want to go out" (i.e. right-side) adapters in the ASP.NET IoC engine ------------------------------
            
            // First, their dependencies
            var webClient = new WebClient();
            services.AddSingleton<IWebClient>(webClient);

            // IProvideAuditoriumLayouts interface
            IProvideAuditoriumLayouts auditoriumSeatingRepository = new AuditoriumWebClient("http://localhost:50950/", webClient);
            services.AddSingleton<IProvideAuditoriumLayouts>(auditoriumSeatingRepository);

            // IProvideCurrentReservations interface
            IProvideCurrentReservations seatReservationsProvider = new SeatReservationsWebClient("http://localhost:50951/", webClient);
            services.AddSingleton<IProvideCurrentReservations>(seatReservationsProvider);

            // Now, the right-side adapter for the IProvideUpToDateAuditoriumSeating right-side port)
            services.AddScoped<IProvideUpToDateAuditoriumSeating, AuditoriumSeatingAdapter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            ConfigureSwagger(app, provider);

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private void ConfigureSwagger(IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                //Build a swagger endpoint for each discovered API version  
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                }
            });
        }
    }
}

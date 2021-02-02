using System;
using System.IO;
using System.Reflection;
using ExternalDependencies;
using ExternalDependencies.AuditoriumLayoutRepository;
using ExternalDependencies.ReservationsProvider;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;

namespace SeatReservations.Api
{
    public class Startup
    {
        private const string ApiContactName = "The seats reservation (transactional) team";
        private const string ApiContactEmail = "contact@fake-reservation-team.com";


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

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

            var reservationsProvider = new ReservationsProvider();

            services.AddSingleton<IProvideCurrentReservations>(reservationsProvider);

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
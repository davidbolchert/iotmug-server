using IoTMug.Api.Providers;
using IoTMug.Data;
using IoTMug.Data.Repositories;
using IoTMug.Services.Implementations;
using IoTMug.Services.Interfaces;
using IoTMug.Services.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace IoTMug.Api
{
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
            // Add entity framework services.
            services.Configure<DatabaseSettings>(Configuration.GetSection(nameof(DatabaseSettings)));
            services.AddDbContext<IoTMugContext>(options => options.UseSqlServer(Configuration.GetSection(nameof(DatabaseSettings))["ConnectionString"]));
            services.AddTransient<IDatabaseService, GenericRepository>();

            // Add certificate services
            services.Configure<CertificateServiceSettings>(Configuration.GetSection(nameof(CertificateServiceSettings)));
            services.AddTransient<ICertificateService, CertificateServiceImplementation>();

            // Add provisionnning services
            services.Configure<ProvisionningServiceSettings>(Configuration.GetSection(nameof(ProvisionningServiceSettings)));
            services.AddTransient<IProvisioningService, ProvisioningServiceImplementation>();

            // Add iot hub services
            services.Configure<IoTHubSettings>(Configuration.GetSection(nameof(IoTHubSettings)));
            services.AddTransient<IIoTHubService, IoTHubServiceImplementation>();

            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));   
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddMvcOptions(options => options.EnableEndpointRouting = false)
                .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore)
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = new LowercaseNamingPolicy();
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName == "Development")
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors("AllowAll");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseMvc();
        }
    }
}

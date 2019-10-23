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
using System.Text.Json;

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
            services.Configure<DatabaseSettings>(Configuration.GetSection("ConnectionStrings"));
            services.AddDbContext<IoTMugContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddTransient<IDatabaseService, GenericRepository>();

            // Add iot hub services
            services.Configure<CertificateServiceSettings>(Configuration.GetSection(nameof(CertificateServiceSettings)));
            services.AddTransient<ICertificateService, CertificateService>();

            services.AddCors();
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

            app.UseCors(builder => 
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyMethod();
                builder.WithHeaders("authorization");
                builder.WithHeaders("content-type");
            });
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseMvc();
        }
    }
}

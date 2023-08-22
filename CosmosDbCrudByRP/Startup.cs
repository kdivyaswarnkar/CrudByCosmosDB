using CosmosDbCrud_DAL.Model;
using CosmosDbCrud_DAL.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Linq;

namespace CosmosDbCrudByRP
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            // Load the main application's configuration (e.g., appsettings.json)
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            // Load the configuration from localsetting.json in the DAL library project
            IConfiguration dalConfiguration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory) // Use the base directory of the main application
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .Build();

            // Combine configuration sources with precedence given to localsetting.json
            configuration = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddConfiguration(configuration)
                .AddConfiguration(dalConfiguration.GetSection("Values"))
                .Build();

            // Register the combined configuration in the DI container
            services.AddSingleton(configuration);
            services.AddRazorPages();
            services.AddSingleton<IEmployeeService, EmployeeService>();      
            services.AddScoped<IBlobStorageService, BlobStorageService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}

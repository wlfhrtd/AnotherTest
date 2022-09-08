using DAL.EfStructures;
using DAL.Repositories;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using MVC.Services;
using System;

namespace MVC
{
    public class Startup
    {
        private readonly IWebHostEnvironment env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            this.env = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContextPool<ApplicationDbContext>(
                options =>
                {
                    options.UseNpgsql(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure().CommandTimeout(60));
                    options.EnableSensitiveDataLogging(true);
                });

            services.AddScoped<IDepartmentRepository, DepartmentRepository>();

            services.AddScoped<IFileManager, FileManager>();
            services.AddScoped<ISyncFromFile, SyncFromFile>();

            // WebOptimizer config
            if (env.IsDevelopment() || env.IsEnvironment("Local"))
            {
                services.AddWebOptimizer(false, false);
            }
            else
            {
                services.AddWebOptimizer(options =>
                {
                    options.MinifyCssFiles(); // ALL
                    // options.MinifyJsFiles(); // ALL
                    options.MinifyJsFiles("js/site.js"); // doesnt add "min" to names; minified versions are not on disk but cached
                    options.MinifyJsFiles("lib/**/*.js"); // compiler may complain on absence

                    options.AddJavaScriptBundle("js/validations/validationCode.js", "js/validations/**/*.js");
                    options.AddJavaScriptBundle(
                        "js/validations/validationCode.js",
                        "js/validations/validators.js",
                        "js/validations/errorFormatting.js"
                        );
                });
            }
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
                app.UseExceptionHandler("/Departments/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Departments}/{action=Index}/{name?}");
            });
        }
    }
}

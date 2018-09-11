using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using BLL;
using Model;
using Microsoft.AspNetCore.Routing.Constraints;

namespace Tn
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            services.Configure<DbOptions>(opts =>
            {
                opts.ConnectionString = Configuration.GetSection("DbConn:ConnectionString").Value;
                opts.Database = Configuration.GetSection("DbConn:Database").Value;
            });

            services.Configure<List<NewsFrom>>(Configuration.GetSection("NewsFrom"));

            services.AddTransient<INewsRepository, NewsRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {

                routes.MapRoute(
                   name: "sitemap",
                   template: "sitemap.xml",
                   defaults: new { controller = "Home", action = "Sitemap" });

                routes.MapRoute(
                   name: "info",
                   template: "info-{Id}.html",
                   defaults: new { controller = "Home", action = "Info" });


                routes.MapRoute(
                   name: "code",
                   template: "{code}/",
                   defaults: new { controller = "Home", action = "Index" });


               
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

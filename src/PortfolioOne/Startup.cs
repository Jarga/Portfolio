using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.Http;
using Microsoft.Framework.DependencyInjection;
using Microsoft.AspNet.Hosting;
using Microsoft.Framework.ConfigurationModel;

namespace PortfolioOne
{
    public static class Global
    {
        public static IConfiguration Configuration { get; set; }
    }
    public class Startup
    {

        public Startup(IHostingEnvironment env)
        {
            // Setup configuration sources.
            Global.Configuration = new Configuration()
                .AddIniFile("config.ini")
                .AddEnvironmentVariables();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseErrorPage();

            // Add static files to the request pipeline.
            app.UseStaticFiles();

            //Use MVC
            app.UseMvc( routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}

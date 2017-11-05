using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DutchTreat
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // ASP.NET Core requires Dependency Injection
            // Here we use the default microsoft provider for dependency injection
            services.AddMvc(); // Injects all services the MVC subsystem needs
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // How should be listen for webrequests
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}

            // Lamda: Anytime a request comes in, write hello world
            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});

            // Order of middleware is important!

            // Finding the index.html file by default
            //app.UseDefaultFiles(); // middleware to look for a default file and redirects the internal path to it
            // Disable UseDefaultFiles, since the controller should handle which files to serve

            // Serve a HTML page to the browser
            // Serve static files 
            app.UseStaticFiles(); // middleware, which by default it serves static files in the wwwroot directory, which should only contain files which are allowed to serve (no configuration files etc.)

            // Middleware to enable MVC
            // With the MVC middleware the app is listening to requests and map them to a MVC controller            
            app.UseMvc(cfg => // Typical pattern in ASP.NET MVC core: Pass in lambda-expression to configure elements
            {
                // Set up routes: MVC will look at the pattern of the URL and map it to a controller
                // This routes requests to the index action of the app controller, if there is no other route for the given URL specified
                cfg.MapRoute("Default", // Name
                    "{controller}/{action}/{id?}", // Template with placeholders
                    new { controller = "App", Action = "Index" }); // Controllers
            });
        }
    }
}

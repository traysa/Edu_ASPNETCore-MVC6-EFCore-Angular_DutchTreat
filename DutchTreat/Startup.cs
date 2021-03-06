﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using DutchTreat.Services;
using DutchTreat.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using AutoMapper;
using DutchTreat.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DutchTreat
{
    public class Startup
    {
        private readonly IConfiguration _config;
        private readonly IHostingEnvironment _env;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public Startup(IConfiguration config, IHostingEnvironment env)
        {
            _config = config;
            _env = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Register the db context as service in the service collection, so it can be injected into different services needed (e.g. inside a controller)
            // The DB provider is given via a configuration lambda
            services.AddDbContext<DutchContext>(cfg =>
            {
                // The provider is given in a configuration file, which has been registered in Program.cs
                cfg.UseSqlServer(_config.GetConnectionString("DutchConnectionString")); // Different databases can be added by other packages
            });

            // Add AutoMapper support
            // AddAutoMapper comes from the NuGet Package "AutoMapper.Extensions.Microsoft.DependencyInjection"
            // Required to define mapping, otherwise the following error appears: "Unmapped members were found."
            services.AddAutoMapper(); 

            // You can add three different types of services
            // - AddTransient: No data on themselves; often methods, which just do things; Lightweight
            // - AddScoped: They are kept for the length of the connection
            // - AddSingleton: Services created once and are kept for the lifetime of the server running
            // Dependency Injection: Define interface and concrete implementation of the service;
            // In this case: When a mailservice is needed, take the concrete implementation (NullMailService) defined here;
            // NullMailService on the otherhand requires a logger. Also here a default implementation is known and does not need to
            // be specified here.
            services.AddTransient<IMailService, NullMailService>();
            // TODO: Support for real mail service

            // By Adding the identity service, cookies are by default used
            services.AddIdentity<StoreUser, IdentityRole>(cfg =>
            {
                cfg.User.RequireUniqueEmail = true;
                //cfg.Password.RequireDigit = true;
            })
            .AddEntityFrameworkStores<DutchContext>(); // define type of context used internally in Identity to get the different objects that are stored in the db; Often Ideneity Context and Data context are separated, but not in this project

            // Define two kinds of authentication to support: Cookies and JWT Tokens 
            services.AddAuthentication()
                .AddCookie()
                .AddJwtBearer(cfg => // Setup the parameters for validating incoming tokens
                {
                    cfg.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = _config["Tokens:Issuer"], // Issuer: Who created the token
                        ValidAudience = _config["Tokens:Audience"], // Audience: Who can use the token
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]))
                    };
                });

            // Call data seeder for database
            services.AddTransient<DutchSeeder>();
            
            // 
            services.AddScoped<IDutchRepository, DutchRepository>();
            //services.AddScoped<IDutchRepository, MockDutchRepository>();

            // ASP.NET Core requires Dependency Injection
            // Here we use the default microsoft provider for dependency injection
            services.AddMvc(opt => // Injects all services the MVC subsystem needs
            {
                if (_env.IsProduction())
                {
                    // Require Https when in Production
                    // Can be also limited on certain controllers and actions
                    opt.Filters.Add(new RequireHttpsAttribute());
                }
            }) 
                .AddJsonOptions(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore); // Trims off self-referencing objects, like produced in the GetAllOrders method in the DutchReporsitory called in the OrderControllers
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // How should be listen for webrequests
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // Show exceptions in the browser window when in development
                // Environment can be configured in the project propeties -> debug
                app.UseDeveloperExceptionPage();
            } else
            {
                // show actual error page on exceptions
                app.UseExceptionHandler("/error");

            }

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

            // Enable authentication and use all configurations configured in ConfigureServices
            // Needs to be before UseMvc
            app.UseAuthentication();

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

            //if (env.IsDevelopment())
            //{
            //    // Seed the Database
            //    // Requires a scope in EF Core 2 to get the Dutch Context
            //    using (var scope = app.ApplicationServices.CreateScope())
            //    {
            //        var seeder = scope.ServiceProvider.GetService<DutchSeeder>();
            //        seeder.Seed().Wait(); // Make synchronize with "Wait", since we cannot make Configure method asynchronize (does not work well in the current version)
            //    }
            //}
        }
    }
}

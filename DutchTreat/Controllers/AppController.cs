using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DutchTreat.Controllers
{
    // Controller maps incoming requests to specific actions
    public class AppController : Controller
    {
        // Create an action, which contains the logic
        // Action result maps logic to a view 
        public IActionResult Index()
        {
            // This action returns a view (you can also return an error or redirect)
            // How does it know which view to call?
            // --> ASP.NET MVC convention: It is looked into the "Views" folder for an folder, 
            //     which is named after the controller (in this case "App"). In this folder
            //     all views for the controller should be found. The View must be named like the
            //     action (in this case "Index").
            // A view represents not an html file, but a razor file
            // Razor is a syntax for generating/replacing/adding small amount of view logic in html
            // with C#
            return View();
        }
    }
}

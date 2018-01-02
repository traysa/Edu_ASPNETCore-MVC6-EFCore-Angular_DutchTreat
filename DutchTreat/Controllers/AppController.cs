using DutchTreat.Data;
using DutchTreat.Services;
using DutchTreat.ViewModels;
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
        private readonly IMailService _mailService;
        public IDutchRepository _repository;

        // Contructor
        // Requirement for the AppController is to know the a mail service for one of its methods; Therefore we inject a mailservice
        // Asking dependency injection layer to construct one of these mail services; This is done and handled in Startup.cs
        public AppController(IMailService mailService, IDutchRepository repository)
        {
            _mailService = mailService;
            _repository = repository;
        }

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

        // Tag helpers (came out with ASP.NET Core and ASP.NET MVC 6)
        // Specifies the url; Instead of "/app/contact", it will be "/contact"
        [HttpGet("contact")] 
        public IActionResult Contact()
        {
            //ViewBag.Title = "Contact us"; // Not required since it is also specified on the contact view

            return View();
        }

        // Tag helper helps MVC to determine which request to process
        // The object model gets data as soon the input tags in the according view have names 
        // Model binding: We bind the model to the ContactViewModel object; the names in the input tags on the razor view must match (case-insensitive)
        [HttpPost("contact")]
        public IActionResult Contact(ContactViewModel model)
        {
            //ViewBag.Title = "Contact us"; // Not required since it is also specified on the contact view

            // Validate Model input
            if (ModelState.IsValid)
            {
                // Send the email
                _mailService.SendMessage("test@test.com", model.Subject, $"From: {model.Name} - {model.Email}, Message: {model.Message}");

                // Response to user
                ViewBag.UserMessage = "Mail sent";
                
                // Clear the form
                ModelState.Clear();
            }
            return View();
        }

        public IActionResult About()
        {
            ViewBag.Title = "About us";
            return View();
        }

        /// <summary>
        /// Shopping page for users
        /// </summary>
        /// <returns>A view with all products</returns>
        public IActionResult Shop()
        {
            // Fluent syntax
            //var results = _context.Products
            //    .OrderBy(p => p.Category)
            //    .ToList();
            //return View(results);

            // LINQ querry
            //var results = from p in _context.Products
            //              orderby p.Category
            //              select p;
            //// Passing data to the view
            //return View(results.ToList());

            // Repository
            var results = _repository.GetAllProducts();
            return View(results);
        }
    }
}

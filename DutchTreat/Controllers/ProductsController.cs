using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using Microsoft.Extensions.Logging;

namespace DutchTreat.Controllers
{
    [Route("api/[Controller]")]
    public class ProductsController : Controller
    {
        private readonly IDutchRepository _repository;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IDutchRepository repository, ILogger<ProductsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                //return Json(_repository.GetAllProducts());
                return Ok(_repository.GetAllProducts()); //Returning a statuscode, independent from the format, e.g. Json
            } catch (Exception ex)
            {
                _logger.LogError($"Failed to get products: {ex}");
                //return null; // empty result; user does not know which error
                //return Json("Bad request");
                return BadRequest("Failed to get products");
            }
        }
    }
}

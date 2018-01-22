using AutoMapper;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DutchTreat.Controllers
{
    // Subcontroller of Orders
    // Since {orderId} is in the route it must be a parameter
    [Route("/api/orders/{orderid}/items")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // No cookies used, only JWT; therefore no redirection to Login page happens on API, instead 401 status error
    public class OrderItemsController : Controller
    {
        private readonly IDutchRepository _repository;
        private readonly ILogger<OrderItemsController> _logger;
        private readonly IMapper _mapper;

        public OrderItemsController(IDutchRepository repository, ILogger<OrderItemsController> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get(int orderId)
        {
            // Since this controller required authorization, the user is known
            var order = _repository.GetOrderById(User.Identity.Name, orderId);
            if (order != null)
                return Ok(_mapper.Map<IEnumerable<OrderItem>,IEnumerable<OrderItemViewModel>>(order.Items));
            return NotFound();
        }

        // Since {orderId} is in the route it must be a parameter
        // Additionally {id} is a parameter.
        [HttpGet("{id}")]
        public IActionResult Get(int orderId, int id)
        {
            // Since this controller required authorization, the user is known
            var order = _repository.GetOrderById(User.Identity.Name, orderId);
            if (order != null)
            {
                var item = order.Items.Where(i => i.Id == id).FirstOrDefault();
                return Ok(_mapper.Map<OrderItem,OrderItemViewModel>(item));
            }
            return NotFound();
        }

        // Post
        // ...

        // Delete
        // ...
    }
}

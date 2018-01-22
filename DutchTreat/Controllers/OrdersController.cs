using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using Microsoft.Extensions.Logging;
using DutchTreat.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;

namespace DutchTreat.Controllers
{
    [Route("api/[Controller]")]
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)] // No cookies used, only JWT; therefore no redirection to Login page happens on API, instead 401 status error
    public class OrdersController : Controller
    {
        private readonly IDutchRepository _repository;
        private readonly ILogger<OrdersController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<StoreUser> _userManager;

        // Constructor, which injects the repository-, logger- and mapper-service
        public OrdersController(IDutchRepository repository, 
            ILogger<OrdersController> logger, 
            IMapper mapper,
            UserManager<StoreUser> userManager)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Get(bool includeitems = true) //Adding a query string 'includeitem'
        {
            try
            {
                // Since this controller required authorization, the user is known
                var username = User.Identity.Name;

                // Without AutoMapper Service
                //return Ok(_repository.GetAllOrders());
                
                // With AutoMapper Service: Take the order and map it to the OrderViewModel
                //return Ok(_mapper.Map<IEnumerable<Order>, IEnumerable<OrderViewModel>>(_repository.GetAllOrders()));
                
                // With AutoMapper Service: Take the order and map it to the OrderViewModel
                //var results = _repository.GetAllOrders(includeitems);
                //return Ok(_mapper.Map<IEnumerable<Order>, IEnumerable<OrderViewModel>>(results));
                
                //
                var results = _repository.GetAllOrdersByUser(username,includeitems);


                return Ok(_mapper.Map<IEnumerable<Order>, IEnumerable<OrderViewModel>>(results));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get orders: {ex}");
                return BadRequest("Failed to get orders");
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            try
            {
                // Since this controller required authorization, the user is known
                var order = _repository.GetOrderById(User.Identity.Name,id);
                if (order != null)
                    // Without AutoMapper Service
                    //return Ok(order);
                    // With AutoMapper Service: Take the order and map it to the OrderViewModel
                    return Ok(_mapper.Map<Order, OrderViewModel>(order));
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get orders: {ex}");
                return BadRequest("Failed to get orders");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]OrderViewModel model) // If tag FromBody is not given the data is tried to get from the url querystring and not from the post body
        {
            // Add to the db
            try
            {
                if (ModelState.IsValid) // Validation is defined in the OrderViewModel
                {
                    // Without AutoMapper convert Order View Model to an order
                    //var newOrder = new Order()
                    //{
                    //    OrderDate = model.OrderDate,
                    //    OrderNumber = model.OrderNumber,
                    //    Id = model.OrderId
                    //};
                    // With AutoMapper convert Order View Model to an order
                    var newOrder = _mapper.Map<OrderViewModel, Order>(model);

                    // Validation if OrderDate has been specified
                    if (newOrder.OrderDate == DateTime.MinValue)
                    {
                        newOrder.OrderDate = DateTime.Now;
                    }

                    // New orders should be assigned to a specific user
                    // User.Identity.Name is just a claim and not the actual user object from the db; therefore we need to get it
                    var currentUser = await _userManager.FindByNameAsync(User.Identity.Name); // due to await the method must return an async Task 
                    newOrder.User = currentUser;

                    _repository.AddEntity(newOrder);
                    if (_repository.SaveAll()) // The SaveAll will safe the new order in the db, where it will gets its Id generated
                    {
                        // Without AutoMapper convert the new order back to a order view model
                        //var vm = new OrderViewModel()
                        //{
                        //    OrderId = newOrder.Id,
                        //    OrderDate = newOrder.OrderDate,
                        //    OrderNumber = newOrder.OrderNumber
                        //};
                        //return Created($"/api/orders/{vm.OrderId}", vm); // You have to return Created instead of Ok when a new object has been created

                        // With AutoMapper convert the new order back to a order view model
                        return Created($"/api/orders/{newOrder.Id}", _mapper.Map<Order,OrderViewModel>(newOrder)); // You have to return Created instead of Ok when a new object has been created
                    }
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to save a new order: {ex}");
            }

            return BadRequest("Failed to save new order");
        }
    }
}

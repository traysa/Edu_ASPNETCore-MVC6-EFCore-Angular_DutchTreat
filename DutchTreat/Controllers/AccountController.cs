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
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace DutchTreat.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly SignInManager<StoreUser> _signInManager;
        private readonly UserManager<StoreUser> _userManager;
        private readonly IConfiguration _config;

        public AccountController(ILogger<OrdersController> logger
            , SignInManager<StoreUser> signInManager
            , UserManager<StoreUser> userManager
            , IConfiguration config)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _config = config;
        }

        public IActionResult Login()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "App"); // Redirect to Action "Index" of Controller "App"
            }

            return View();            
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid) //Validates input according to the view model
            {
                // Login by calling sign in manager; Method "PasswordSignInAsync" allows to login with only
                // Username and Password instead of getting the StoreUser object
                // Returns a task
                var result = await _signInManager.PasswordSignInAsync(model.Username,
                    model.Password,
                    model.RememberMe,
                    false); // If logon on failure equals true: allows to logout the account if username or password are not correct

                if (result.Succeeded)
                {
                    if (Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        // Redirect to original requested page if available in the url query
                        Redirect(Request.Query["ReturnUrl"].First());
                    }
                    else
                    {
                        RedirectToAction("Shop", "App");
                    }                    
                }
            }

            // Error on the entire model (therefore key parameter is an empty string)  
            ModelState.AddModelError("", "Failed to login");

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "App"); // Redirect to Action "Index" of Controller "App"           
        }

        [HttpPost] // Http Post because we do not want to have credentials in the header or query string and Http Get does not allow to put things in the body
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid) // Check if input is valid
            {
                // Sign in with Cookie
                //var result = _signInManager.PasswordSignInAsync(...);

                // Check if username is known
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user != null)
                {
                    // Check password and no lockout for failure
                    var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                    if (result.Succeeded)
                    {
                        // Create the token
                        // 1. Get claims: Claims are statements about an entity (typically, the user) and additional metadata
                        var claims = new[]
                        {
                            // Type = Subject;
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            // JTI is a unique string for each token
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            // UniqueName is the the username, mapped to the idenity inside the user object in every controller
                            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
                        };

                        // 2. Create key: Secret to encrypt token
                        // Some parts of the token are encrypted (e.g. user credentials), others not (e.g. expiration date)
                        // The Key-string is read from the configuration, so it can be accessed from other parts of the code and can be replaced later by people setting up the system
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));

                        // 3. Signing credentails
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        // Create JWT token specification
                        var token = new JwtSecurityToken(
                            _config["Tokens:Issuer"],
                            _config["Tokens:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddMinutes(20), // could be also made configurable
                            signingCredentials: creds
                            );

                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token), // Create token (created as string)
                            expiration = token.ValidTo // Expose expiration time of token
                        };

                        return Created("", results);
                    }
                }
            }

            return BadRequest();
        }
    }
}
